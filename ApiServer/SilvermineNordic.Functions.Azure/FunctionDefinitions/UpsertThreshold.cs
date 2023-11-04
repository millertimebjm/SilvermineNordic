using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SilvermineNordic.Models;
using SilvermineNordic.Repository.Services;

namespace SilvermineNordic.Functions.Azure
{
    public class ThresholdUpsert
    {
        private readonly ILogger _logger;
        private readonly IRepositoryThreshold _thresholdService;

        public ThresholdUpsert(
            ILoggerFactory loggerFactory,
            IRepositoryThreshold thresholdService)
        {
            _logger = loggerFactory.CreateLogger<ThresholdGet>();
            _thresholdService = thresholdService;
        }

        // http://localhost:7071/api/ThresholdUpsert/
        // http://localhost:7071/api/ThresholdUpsert/
        // curl -i -X POST -H 'Content-Type: application/json' -d '{"Id":"0", "TemperatureInCelciusHighThreshold": "10.0", "TemperatureInCelciusLowThreshold": "0.0", "HumidityHighThreshold", "11.0", "HumidityLowThreshold": "1.0"}' http://localhost:7071/api/ThresholdUpsert
        [Function("UpsertThreshold")]
        public async Task<HttpResponseData> RunAsync(
                [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
                Threshold threshold)
        {
            threshold = await _thresholdService.UpsertThreshold(threshold);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(threshold);
            return response;
        }
    }
}
