using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SilvermineNordic.Models;
using SilvermineNordic.Repository.Services;

namespace SilvermineNordic.Functions.Azure
{
    public class ThresholdGet
    {
        private readonly ILogger _logger;
        private readonly IRepositoryThreshold _thresholdService;

        public ThresholdGet(
            ILoggerFactory loggerFactory,
            IRepositoryThreshold thresholdService)
        {
            _logger = loggerFactory.CreateLogger<SensorReadingEvent>();
            _thresholdService = thresholdService;
        }

        // http://localhost:7071/api/ThresholdGet
        [Function("ThresholdGet")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            var thresholds = await _thresholdService.GetThresholds();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(thresholds);
            return response;
        }
    }
}
