using IncidentEmailService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IncidentEmailService;

public class GetIncidents
{
    private readonly ILogger<GetIncidents> _logger;

    public GetIncidents(ILogger<GetIncidents> logger)
    {
        _logger = logger;
    }

    [Function("GetIncidentData")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        // Read and deserialize the request body into the Incident object
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<Incident>(requestBody);


        return new OkObjectResult(new { message = "Data processed successfully", receivedData = data });
    }
}