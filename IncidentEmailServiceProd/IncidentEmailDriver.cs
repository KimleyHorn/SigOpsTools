using System.Net;
using IncidentEmailServiceProd.Controllers;
using IncidentEmailServiceProd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IncidentEmailServiceProd
{
    public class IncidentEmailDriver
    {
        private readonly ILogger<IncidentEmailDriver> _logger;
        private readonly CrashController _crashController;
        private readonly CrashDataAccessLayer _crashDataAccessLayer;
        private readonly IIncidentRepository _incRepository;

        public IncidentEmailDriver(ILoggerFactory loggerFactory, IIncidentRepository incRepository)
        {
            _incRepository = incRepository;
            _logger = loggerFactory.CreateLogger<IncidentEmailDriver>();
            //_logger = new Logger<IncidentEmailDriver>();
            _crashDataAccessLayer = new CrashDataAccessLayer();
            _crashController = new CrashController(loggerFactory.CreateLogger<CrashController>(), _incRepository);
        }

        [Function("IncidentEmail")]
        public async Task<IActionResult> IncidentEmail([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            if (req == null)
            {
                _logger.LogError("HTTP request is null.");
                return new BadRequestObjectResult("HTTP request is null.");
            }
            
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Incident>(requestBody);
            if (data != null)
            {
                await _crashController.NewCrash(data);
            }
            return new OkObjectResult(new { message = "Data processed successfully", receivedData = data });
           
        }
    }
}
