using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SilvermineNordic.Models;
using SilvermineNordic.Repository.Services;

namespace SilvermineNordic.Functions.Azure
{
    public class ReadingGet
    {
        private readonly ILogger _logger;
        private readonly IRepositoryReading _readingService;

        public ReadingGet(
            ILoggerFactory loggerFactory,
            IRepositoryReading readingService)
        {
            _logger = loggerFactory.CreateLogger<ReadingGet>();
            _readingService = readingService;
        }

        // http://localhost:7071/api/ReadingGet/Sensor/
        // http://localhost:7071/api/ReadingGet/Sensor/5
        // http://localhost:7071/api/ReadingGet/Weather/5
        // http://localhost:7071/api/ReadingGet/Weather/5/5
        // http://localhost:7071/api/ReadingGet/Sensor/5/10
        [Function("ReadingGet")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ReadingGet/{readingType}/{count:int?}/{skip:int?}")] HttpRequestData req,
            string readingType,
            int? count,
            int? skip)
        {
            var countNonNull = count ?? 1;
            countNonNull = countNonNull > 100 ? 100 : countNonNull;
            countNonNull = countNonNull < 1 ? 1 : countNonNull;
            var skipNonNull = skip ?? 0;
            var readings = await _readingService.GetLastNReadingAsync(
                Enum.Parse<ReadingTypeEnum>(readingType, ignoreCase: true),
                countNonNull,
                skipNonNull);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(readings);
            return response;
        }
    }
}
