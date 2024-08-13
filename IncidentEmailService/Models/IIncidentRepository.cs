namespace IncidentEmailService.Models
{
    public interface IIncidentRepository
    {
        Task<IEnumerable<Incident>> GetAllIncidentsAsync();
        Task<Incident> GetIncidentByIdAsync(string id);
        Task<IEnumerable<Incident>> GetIncidentsByRoadwayNameAsync(string roadwayName);
        Task<IEnumerable<Incident>> GetIncidentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Incident>> GetIncidentsBySubtypeAsync(string subtype);
        Task<IEnumerable<Incident>> GetIncidentsByLanesAffectedAsync(string lanesAffected);
        Task<IEnumerable<Incident>> RecordCrashUpdateAsync(Incident incident);
        Task AddIncidentAsync(Incident incident);
        Task UpdateIncidentAsync(Incident incident);
        Task DeleteIncidentAsync(int id);
    }
}
