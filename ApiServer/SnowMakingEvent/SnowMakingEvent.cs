using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Azure.Data.Tables;
using Azure;
using SnowMakingEvent.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace SnowMakingEvent
{
    public class SnowMakingEvent
    {
        private const decimal HumidityLowThreshold = 20;
        private const decimal HumidityHighThreshold = 30;
        private const decimal TemperatureInCelciusLowThreshold = 20;
        private const decimal TemperatureInCelciusHighThreshold = 30;
        private const string TemperatureInCelciusQueryParameterName = "temperatureInCelcius";
        private const string HumidityQueryParameterName = "humidity";
        private readonly TableClient _client;

        public SnowMakingEvent()
        {
            _client = new TableClient("[redacted]", "SnowMaking");
        }

        // http://localhost:7077/api/SnowMakingEvent?temperatureInCelcius=25&humidity=25
        // http://localhost:7077/api/SnowMakingEvent?temperatureInCelcius=15&humidity=15
        [FunctionName("SnowMakingEvent")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var temperatureInCelciusString = req.Query[TemperatureInCelciusQueryParameterName].ToString();
            log.LogInformation($"{TemperatureInCelciusQueryParameterName} value entered as {temperatureInCelciusString}.");
            var humidityString = req.Query[HumidityQueryParameterName];
            log.LogInformation($"{HumidityQueryParameterName} value entered as {humidityString}.");

            if (decimal.TryParse(temperatureInCelciusString, out var temperatureInCelcius)
                && decimal.TryParse(humidityString, out var humidity))
            {
                try
                {
                    var sensorData = await GetTableData();

                    var isInZoneBefore = IsInZone(decimal.Parse(sensorData.LastTemperatureInCelcius), decimal.Parse(sensorData.LastHumidity));
                    var isInZoneAfter = IsInZone(temperatureInCelcius, humidity);
                    if (isInZoneBefore != isInZoneAfter)
                    {
                        log.LogInformation("Threshold for notification has been reached!");
                    }
                    else
                    {
                        log.LogInformation("Threshold for notification NOT reached!");
                    }

                    var response = await SetTableData(sensorData, temperatureInCelciusString, humidityString);
                    log.LogInformation($"Storage Table Update Response: {response.Status} - {response.ReasonPhrase} - {response.Content}");

                    return new OkObjectResult("Event processed.");
                }
                catch (Exception ex)
                {
                    return new BadRequestObjectResult(ex.Message);
                }
            }

            return new BadRequestObjectResult("Query parameters not formatted correctly.");
        }

        private async Task<Response> SetTableData(SnowMakingModel model, string temperatureInCelciusQueryParameter, string humidityQueryParameter)
        {
            model.LastTemperatureInCelcius = temperatureInCelciusQueryParameter;
            model.LastHumidity = humidityQueryParameter;
            model.LastReading = DateTime.UtcNow;
            return await _client.UpdateEntityAsync(model, model.ETag);
        }

        private async Task<SnowMakingModel> GetTableData()
        {
            var result = _client.QueryAsync<SnowMakingModel>();
            await foreach (var row in result)
            {
                return row;
            }
            return null;
        }

        private static bool IsInZone(decimal lastTemperatureInCelcius, decimal lastHumidity)
        {
            if (lastTemperatureInCelcius > TemperatureInCelciusLowThreshold 
                && lastTemperatureInCelcius < TemperatureInCelciusHighThreshold
                && lastHumidity > HumidityLowThreshold
                && lastHumidity < HumidityHighThreshold)
            {
                return true;
            }
            return false;
        }
    }
}
