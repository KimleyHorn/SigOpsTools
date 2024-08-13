using System.Configuration;
using IncidentEmailService.Controllers;
using IncidentEmailService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IncidentEmailService;

public class Program
{
    public static void Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                // Example of using ConfigurationManager to access app.config settings
                var someSetting = ConfigurationManager.AppSettings["SomeSettingKey"];
                Console.WriteLine($"Config value: {someSetting}");

                // Add other configurations as needed
            })
            .ConfigureFunctionsWorkerDefaults(worker =>
            {
                worker.UseFunctionExecutionMiddleware(); // Custom middleware
            })
            .ConfigureServices((context, services) =>
            {
                // Adding services and dependencies
                services.AddLogging();
                services.AddControllers();
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new() { Title = "SigOpsTools.API", Version = "v1" });
                });

                services.AddEndpointsApiExplorer();
                services.AddScoped<IIncidentRepository, CrashDataAccessLayer>();
                services.AddScoped<CrashController>();
            }).ConfigureFunctionsWebApplication()
            .ConfigureLogging((context, logging) =>
            {
                logging.AddConsole();
                logging.AddDebug();
            })
            .Build();

        host.Run();
    }
}