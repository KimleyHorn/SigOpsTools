using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using SigOpsTools.API.Models;

namespace SigOpsTools.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CrashController : ControllerBase
    {
        private readonly ILogger<CrashController> _logger;
        private readonly IIncidentRepository _incidentRepository;
        private CrashDataAccessLayer _crashDataAccessLayer;

        public CrashController(ILogger<CrashController> logger, IIncidentRepository incidentRepository)
        {
            _logger = logger;
            _incidentRepository = incidentRepository;
            _crashDataAccessLayer = new CrashDataAccessLayer();
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

        [HttpPost(Name = "RecordCrashUpdate")]
        public IActionResult Post([FromBody] Incident incident)
        {
            try
            {
                if (incident == null)
                {
                    return BadRequest();
                }

                _crashDataAccessLayer.RecordCrashUpdateAsync(incident);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while recording the crash update.");
                throw;
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
