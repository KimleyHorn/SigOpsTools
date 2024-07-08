using Microsoft.AspNetCore.Mvc;

namespace SigOpsTools.API.Controllers
{
    public class CrashController
    {

        private readonly ILogger<Incident> _logger;

        public CrashController(ILogger<Incident> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetCrashes")]
        public IEnumerable<Incident> Get()
        {
            try
            {
                var ctx = 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            return Enumerable.Range(1, 5).Select(index => new Incident
                {
                    Id = 0,
                    RoadwayName = null,
                    DateReported = null,
                    DateUpdated = default,
                    Description = null,
                    Location = null,
                    DirectionOfTravel = null,
                    EventType = null,
                    EventSubtype = null,
                    Detours = null,
                    LanesAffected = null
                })
            .ToArray();
        }
    }
}
