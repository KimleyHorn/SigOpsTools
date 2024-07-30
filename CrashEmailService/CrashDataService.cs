using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CrashEmailService
{
    public class CrashDataService
    {
        private readonly ILogger<CrashDataService> _logger;

        public CrashDataService(ILogger<CrashDataService> logger)
        {
            _logger = logger;
        }

        [Function("NewCrash")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
