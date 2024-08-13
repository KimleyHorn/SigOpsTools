﻿using System.Data;
using IncidentEmailService.Models;
using MySqlConnector;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace IncidentEmailService
{
    public class CrashDataAccessLayer : IIncidentRepository
    {
        internal static readonly string MySqlDbName = ConfigurationManager.AppSettings["DB_NAME"] ?? "mark1";
        internal static readonly string? MySqlConnString = ConfigurationManager.AppSettings["CONN_STRING"];
        internal static MySqlConnection MySqlConnection = null!;

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
        static CrashDataAccessLayer()
        {
            try
            {
                MySqlConnection ??= new MySqlConnection(MySqlConnString);
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
            finally
            {
                MySqlConnection.Close();
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
                    roadway_name = roadwayName
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
                    subtype = subtype
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
                    lanes_affected = lanesAffected
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
                    reported = DateTime.Now
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        public static async Task<List<(string, string)>> SendTo(Incident i)
        {

            var data = new Dictionary<(int, int), string>();
            var sendList = new List<(string, string)>();

            try
            {
                if (MySqlConnection.State == ConnectionState.Closed)
                    await MySqlConnection.OpenAsync();

                var cmd = MySqlConnection.CreateCommand();

                // Using a parameterized query to prevent SQL injection
                cmd.CommandText = $"SELECT * FROM {MySqlDbName}.crash_data_emails WHERE Region LIKE CONCAT('%', @Region, '%')";
                string regionParameter;

                //Determine if admin should get this email based on severity

                // Determine the region parameter based on the input
                switch (i.region)
                {
                    case "None":
                        regionParameter = "All Regions";
                        break;
                    case "Region 1":
                    case "Region 2":
                    case "Region 3":
                    case "Region 4":
                    case "Region 5":
                        regionParameter = i.region;
                        break;
                    default:
                        regionParameter = "All Regions";
                        break;
                }

                // Add the region parameter to the command
                cmd.Parameters.AddWithValue("@Region", regionParameter);

                if(i.lanes_affected == "All Lanes Blocked.")
                {
                    cmd.CommandText += " AND Title = 'Admin'";
                }

                await using var reader = await cmd.ExecuteReaderAsync();
                var rowIndex = 0;

                while (await reader.ReadAsync())
                {
                    // Iterate through each column in the current row
                    for (var columnIndex = 0; columnIndex < reader.FieldCount; columnIndex++)
                    {
                        // Get the value from the current column
                        var value = reader.GetValue(columnIndex);

                        // Convert the value to a string representation (handling DBNull)
                        var valueStr = value != DBNull.Value ? value.ToString() : null;

                        // Insert the row and column index along with the value into the dictionary
                        data[(rowIndex, columnIndex)] = valueStr;
                    }

                    // Increment the row index for the next row
                    rowIndex++;
                }
                sendList = data
                    .GroupBy(x => x.Key.Item1) // Group by the first integer
                    .Where(g => g.Any(x => x.Key.Item2 == 0) && g.Any(x => x.Key.Item2 == 2)) // Only groups that have both 0 and 2
                    .Select(g => (
                        g.First(x => x.Key.Item2 == 0).Value, // Get the value where second int is 0
                        g.First(x => x.Key.Item2 == 2).Value  // Get the value where second int is 2
                    ))
                    .ToList();


            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                // Ensure the connection is closed
                if (MySqlConnection.State == ConnectionState.Open)
                    await MySqlConnection.CloseAsync();
            }

            return sendList;
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
