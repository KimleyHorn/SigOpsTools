using IncidentEmailService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IncidentEmailService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CrashController : ControllerBase
    {
        private readonly ILogger<CrashController> _logger;
        private readonly IIncidentRepository _incidentRepository;
        private CrashDataAccessLayer _crashDataAccessLayer;
        private EmailSender _emailSender;




        public CrashController(ILogger<CrashController> logger, IIncidentRepository incidentRepository)
        {
            _logger = logger;
            _incidentRepository = incidentRepository;
            _crashDataAccessLayer = new CrashDataAccessLayer();
            _emailSender = new EmailSender();


        }

        [HttpGet("GetAll",Name = "GetAllCrashes")]
        public async Task<IEnumerable<Incident>> Get()
        {
            try
            {
                var incidents = await _crashDataAccessLayer.GetAllIncidentsAsync();
                return incidents;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while getting all incidents.");
                throw;
            }
        }

        [HttpGet("GetById", Name = "GetCrashById")]
        public async Task<ActionResult<Incident>> GetById(string id)
        {
            try
            {
                var incident = await _crashDataAccessLayer.GetIncidentByIdAsync(id);
                if (incident == null)
                {
                    return NotFound();
                }
                return Ok(incident);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occurred while getting the incident with ID {id}.");
                throw;
            }
        }

        [HttpGet("GetByRoadwayName", Name = "GetCrashesByRoadwayName")]
        public async Task<IEnumerable<Incident>> GetByRoadwayName(string roadwayName)
        {
            try
            {
                var incidents = await _crashDataAccessLayer.GetIncidentsByRoadwayNameAsync(roadwayName);
                return incidents;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while getting incidents by roadway name.");
                throw;
            }
        }

        [HttpGet("GetByDateRange", Name = "GetCrashesByDateRange")]
        public async Task<IEnumerable<Incident>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var incidents = await _crashDataAccessLayer.GetIncidentsByDateRangeAsync(startDate, endDate);
                return incidents;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while getting incidents by date range.");
                throw;
            }
        }

        [HttpGet("GetBySubtype", Name = "GetCrashesBySubtype")]
        public async Task<IEnumerable<Incident>> GetBySubtype(string subtype)
        {
            try
            {
                var incidents = await _crashDataAccessLayer.GetIncidentsBySubtypeAsync(subtype);
                return incidents;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while getting incidents by subtype.");
                throw;
            }
        }

        [HttpGet("GetByLanesAffected", Name = "GetCrashesByLanesAffected")]
        public async Task<IEnumerable<Incident>> GetByLanesAffected(string lanesAffected)
        {
            try
            {
                var incidents = await _crashDataAccessLayer.GetIncidentsByLanesAffectedAsync(lanesAffected);
                return incidents;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while getting incidents by lanes affected.");
                throw;
            }
        }

        [HttpPost(template: "NewCrashDetected", Name = "NewCrash")]
        public async Task<IActionResult> NewCrash([FromBody] Incident incident)
        {
            try
            {
                if (incident == null)
                {
                    return BadRequest();
                }

                //var newIncidents = await _crashDataAccessLayer.RecordCrashUpdateAsync(incident);
                var e = new EmailSender();
                var sendList = await CrashDataAccessLayer.SendTo(incident);
                foreach (var i in sendList)
                {
                    var email = await e.SendGridEmailAsync(i, incident);
                   return email ? Ok() : NotFound();
                }

                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while recording the crash update.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }
        [HttpPost(template: "CrashUpdated", Name = "UpdateCrash")]
        public async Task<IActionResult> UpdateCrash([FromBody] Incident incident)
        {
            try
            {
                if (incident == null)
                {
                    return BadRequest();
                }

                //var newIncidents = await _crashDataAccessLayer.RecordCrashUpdateAsync(incident);
                var e = new EmailSender();
                var sendList = await CrashDataAccessLayer.SendTo(incident);
                foreach (var i in sendList)
                {
                    var email = await e.SendGridEmailAsync(i, incident);
                   return email ? Ok() : NotFound();
                }

                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while recording the crash update.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        [Authorize]
        [HttpPut(Name = "AddCrashRecord")]
        public IActionResult Put([FromBody] Incident incident)
        {
            try
            {
                if (incident == null)
                {
                    return BadRequest();
                }

                _incidentRepository.AddIncidentAsync(incident);
                return CreatedAtRoute("GetCrashById", new { id = incident.ID }, incident);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while adding the crash record.");
                throw;
            }
        }
    }
}
