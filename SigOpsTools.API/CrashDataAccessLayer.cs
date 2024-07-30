using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using MySqlConnector;

using System.Data;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using SigOpsTools.API.Models;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace SigOpsTools.API
{
    public class CrashDataAccessLayer : IIncidentRepository
    {
        internal static readonly string MySqlDbName = ConfigurationManager.AppSettings["DB_NAME"] ?? "mark1";
        internal static readonly string? MySqlConnString = ConfigurationManager.AppSettings["CONN_STRING"];
        internal static MySqlConnection MySqlConnection;

        public CrashDataAccessLayer()
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

                if (i != null)
                {
                    foreach (var prop in properties)
                    {
                        var value = prop.GetValue(i);
                        if (value == null ||
                            value is string str && string.IsNullOrEmpty(str) ||
                            value is double and 0 ||
                            value is DateTime d && d == DateTime.MinValue) continue;
                        whereClauses.Add($"{prop.Name} = @{prop.Name}");
                        cmd.Parameters.AddWithValue($"@{prop.Name}", value);
                    }


                }
                cmd.CommandText = $"SELECT * FROM {MySqlDbName}.incidents";
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

        private async Task<IEnumerable<Incident>> GetByRange(string dateColumn, List<DateTime> dates)
        {
            var incidents = new List<Incident>();
            if (MySqlConnection.State == ConnectionState.Closed)
                await MySqlConnection.OpenAsync();

            var inClause = string.Join(",", dates);

            var cmd = MySqlConnection.CreateCommand();
            if (dateColumn != "DateReported" || dateColumn != "LastUpdated") return incidents;

            cmd.CommandText = $"SELECT * FROM {MySqlDbName}.incidents WHERE {dateColumn} IN ({inClause})";
            return incidents;
        }

        public async Task<IEnumerable<Incident>> GetAllIncidentsAsync()
        {
            try
            {
                return await GetByFilter();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public async Task<Incident> GetIncidentByIdAsync(string id)
        {
            try
            {
                var i = await GetByFilter(new Incident
                {
                    ID = id
                });

                return i.FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e + $" Incident with ID: {id} does not exist. Please try again");
                throw;
            }

            
        }

        public async Task<IEnumerable<Incident>> GetIncidentsByRoadwayNameAsync(string roadwayName)
        {
            try
            {
                return await GetByFilter(new Incident
                {
                    RoadwayName = roadwayName
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Task<IEnumerable<Incident>> GetIncidentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Incident>> GetIncidentsBySubtypeAsync(string subtype)
        {
            try
            {
                return await GetByFilter(new Incident
                {
                    Subtype = subtype
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IEnumerable<Incident>> GetIncidentsByLanesAffectedAsync(string lanesAffected)
        {
            try
            {
                return await GetByFilter(new Incident
                {
                    LanesAffected = lanesAffected
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Task<IEnumerable<Incident>> RecordCrashUpdateAsync(Incident incident)
        {
            try
            {
                return GetByFilter(new Incident
                {
                    DateReported = DateTime.Now
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

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

        public string CreateEmail(IEnumerable<Incident> incident)
        {
            //return 
            throw new NotImplementedException();
        }
    }
}
