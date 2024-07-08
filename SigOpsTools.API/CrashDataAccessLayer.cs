using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using MySqlConnector;
using System.Configuration;
using System.Data;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace SigOpsTools.API
{
    public class CrashDataAccessLayer
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


        /// <summary>
        /// A helper method that handles writing the entire DataTable to a MySQL table using MySqlBulkCopy
        /// </summary>
        /// <param name="mySqlTableName">The name of the MySQL table derived from the app.config file</param>
        /// <param name="dataTable">A collection of objects to be written to the MySQL table</param>
        /// <returns>True if operation was successful</returns>
        internal static async Task<bool> MySqlWriter(string mySqlTableName, DataTable dataTable)
        {
            //Write a conditional statement that returns true if the data was written to the table successfully
            try
            {
                await MySqlConnection.OpenAsync();
#if DEBUG
                Console.WriteLine("Connection Opened");
#endif
                var bulkCopy = new MySqlBulkCopy(MySqlConnection)
                {
                    DestinationTableName = $"{MySqlDbName}.{mySqlTableName}"
                };
#if DEBUG
                Console.WriteLine("Bulk Copy Created");
#endif

                await bulkCopy.WriteToServerAsync(dataTable);
#if DEBUG
                Console.WriteLine("Bulk Copy Written");
#endif

                await MySqlConnection.CloseAsync();
#if DEBUG
                Console.WriteLine("Connection Closed");
                Console.WriteLine("Written to Database");
#endif
            }
            catch (NullReferenceException n)
            {
                Console.WriteLine(n + " MySqlConnection object null");
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return true;
        }


        /// <summary>
        /// A helper method that will check the database for a given date range to see if the data is already present. This method will return true if the data is present and false if the data is not present.
        /// </summary>
        /// <param name="mySqlTableName">The name of the MySQL table that the method is checking</param>
        /// <param name="mySqlColName">The column name that contains dates on the MySQL table</param>
        /// <param name="mySqlDbName">The name of the database that the method is checking</param>
        /// <param name="startDate">The first day this method searches for</param>
        /// <param name="endDate">The last day this method searches for</param>
        /// <returns>True if data is present and false if data is not present</returns>
        private async Task<bool> CheckDB(string mySqlTableName, string mySqlColName, string mySqlDbName, DateTime startDate = new DateTime(),
            DateTime endDate = new DateTime())
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be before start date");

            if (startDate == new DateTime())
                startDate = DateTime.Today.AddDays(-1);

            if (endDate == new DateTime())
                endDate = DateTime.Today;

            try
            {
                if (MySqlConnection.State == ConnectionState.Closed)
                    await MySqlConnection.OpenAsync();

                await using var cmd = MySqlConnection.CreateCommand();
                cmd.CommandText =
                    $"SELECT * FROM {mySqlDbName}.{mySqlTableName} WHERE {mySqlColName} BETWEEN @StartDate AND @EndDate";
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate.AddDays(1));

                await using var reader = await cmd.ExecuteReaderAsync();

                if (!reader.HasRows)
                {
                    await MySqlConnection.CloseAsync();
                    return false;
                }

                while (await reader.ReadAsync())
                {
                    var signalEvent = new BaseEventLogModel
                    {
                        Timestamp = reader.GetDateTime("Timestamp"),
                        SignalID = reader.GetInt64("signalID"),
                        EventCode = reader.GetInt64("EventCode"),
                        EventParam = reader.GetInt64("EventParam")
                    };
                    SignalEvents.Add(signalEvent);
                }

                await MySqlConnection.CloseAsync();
            }
            catch (MySqlException t)
            {
                Console.WriteLine("MySQL connection timed out please check on connection health");
                Console.WriteLine(t);
                return false;
            }
            catch (Exception e)
            {
                await WriteToErrorLog("SigOpsMetricsCalcEngine.BaseDataAccessLayer", "CheckDB", e);
                throw;
            }

            return true;
        }

        /// <summary>
        /// A method that inputs a list of valid dates and a list of signal Ids, event codes, start date, end date, and a table name and returns a list of valid dates that are not present in the database
        /// </summary>
        /// <param name="startDate">The first day this method searches for</param>
        /// <param name="endDate">The last day this method searches for</param>
        /// <param name="eventCodes">A list of event codes this method filters by</param>
        /// <param name="mySqlTableName">The name of the MySQL table that the method is checking</param>
        /// <returns>A list of valid dates that are not present in the database</returns>
        internal async Task<List<DateTime>> FillData(DateTime startDate, DateTime endDate, List<long?> eventCodes, string mySqlTableName)
        {
            var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                     .Select(offset => startDate.AddDays(offset));
            await CheckDB(mySqlTableName, "Timestamp", MySqlDbName, startDate, endDate);

            var filteredSignals = allDates
                .Where(date => !SignalEvents.Any(signalEvent => date.Day.Equals(signalEvent.Timestamp.Day) && eventCodes.Contains(signalEvent.EventCode)))
                .ToList();
            return filteredSignals;
        }
    }
}
