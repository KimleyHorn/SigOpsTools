using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using MySqlConnector;
using System.Configuration;
using System.Data;
using SigOpsTools.API.Models;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace SigOpsTools.API
{
    public class CrashDataAccessLayer : IIncidentRepository
    {
        internal static readonly string MySqlDbName = ConfigurationManager.AppSettings["DB_NAME"] ?? "mark1";
        internal static readonly string? MySqlConnString = ConfigurationManager.AppSettings["CONN_STRING"];
        internal static MySqlConnection MySqlConnection;

        static CrashDataAccessLayer()
        {
            try
            {
                MySqlConnection = new MySqlConnection(MySqlConnString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MySqlConnection = new MySqlConnection(null);

            }

        }

        private static async Task<IEnumerable<Incident>> GetByFilter(Incident? i = null)
        {
            var incidents = new List<Incident>();

            try
            {
                if (MySqlConnection.State == ConnectionState.Closed)
                    await MySqlConnection.OpenAsync();

                var cmd = MySqlConnection.CreateCommand();
                var whereClauses = new List<string>();

                var properties = typeof(Incident).GetProperties();
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(i);
                    if (value == null || value is string str && string.IsNullOrEmpty(str)) continue;
                    whereClauses.Add($"{prop.Name} = @{prop.Name}");
                    cmd.Parameters.AddWithValue($"@{prop.Name}", value);
                }

                cmd.CommandText = "SELECT * FROM incidents";
                if (whereClauses.Any())
                {
                    cmd.CommandText += " WHERE " + string.Join(" AND ", whereClauses);
                }

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var incident = new Incident();
                    foreach (var prop in properties)
                    {
                        var value = reader[prop.Name];
                        if (value != DBNull.Value)
                        {
                            prop.SetValue(incident, value);
                        }
                    }
                    incidents.Add(incident);
                }

                return incidents;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return incidents;
            }
        }

        public async Task<IEnumerable<Incident>> GetAllIncidentsAsync()
        {
            return await GetByFilter();
        }

        public async Task<Incident> GetIncidentByIdAsync(string id)
        {
            try
            {
                var i = await GetByFilter(new Incident
                {
                    Id = id
                });

                return i.FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e + $" Incident with ID: {id} does not exist. Please try again");
                throw;
            }

            
        }

        public Task<IEnumerable<Incident>> GetIncidentsByRoadwayNameAsync(string roadwayName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Incident>> GetIncidentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Incident>> GetIncidentsBySubtypeAsync(string subtype)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Incident>> GetIncidentsByLanesAffectedAsync(string lanesAffected)
        {
            throw new NotImplementedException();
        }

        public Task RecordCrashUpdateAsync(Incident incident)
        {
            throw new NotImplementedException();
        }

        public Task AddIncidentAsync(Incident incident)
        {
            throw new NotImplementedException();
        }

        public Task UpdateIncidentAsync(Incident incident)
        {
            throw new NotImplementedException();
        }

        public Task DeleteIncidentAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
