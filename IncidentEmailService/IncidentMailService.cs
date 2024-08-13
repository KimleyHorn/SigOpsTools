using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace IncidentEmailService
{
    public class IncidentMailService
    {
        private readonly ILogger<IncidentMailService> _logger;

        public IncidentMailService(ILogger<IncidentMailService> logger)
        {
            _logger = logger;
        }

        [Function("IncidentMailService")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
