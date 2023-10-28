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
            _logger = loggerFactory.CreateLogger<ThresholdGet>();
            _thresholdService = thresholdService;
        }

        // http://localhost:7071/api/ThresholdGet/5
        // http://localhost:7071/api/ThresholdGet/5/5
        [Function("ThresholdGet")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ThresholdGet/{count:int?}/{skip:int?}")] HttpRequestData req,
            int? count,
            int? skip)
        {
            var thresholds = await _thresholdService.GetThresholds(count, skip);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(thresholds);
            return response;
        }
    }
}
