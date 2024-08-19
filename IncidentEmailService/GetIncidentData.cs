using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SigOpsTools.API.Controllers;
using SigOpsTools.API.Models;

namespace IncidentEmailService
{
    public class GetIncidentData
    {
        private IIncidentRepository _incidentRepository;
        private readonly CrashController _crashController;

        public GetIncidentData(IIncidentRepository incidentRepository, CrashController crashController)
        {
            _incidentRepository = incidentRepository;
            _crashController = crashController;
        }

        [FunctionName("GetIncidentData")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            //log.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var incident = JsonConvert.DeserializeObject<Incident>(requestBody);

            if (incident == null)
            {
                return new BadRequestObjectResult("Invalid request payload");
            }

            var response = _crashController.NewCrash(incident);
            return new OkObjectResult(response);
        }

    }
}
