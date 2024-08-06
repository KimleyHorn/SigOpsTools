using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SigOpsTools.API.Models;

namespace IncidentEmailService
{
    public class MyFunction
    {
        private readonly IIncidentRepository _incidentRepository;

        public MyFunction(IIncidentRepository incidentRepository)
        {
            _incidentRepository = incidentRepository;
        }

        [FunctionName("GetIncidentData")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var incidents = _incidentRepository.RecordCrashUpdateAsync();  // Example method from your repository
            return new OkObjectResult(incidents);
        }
    }
}
