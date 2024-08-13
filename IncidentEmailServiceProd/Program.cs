using IncidentEmailServiceProd.Controllers;
using IncidentEmailServiceProd.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Extensions;


namespace IncidentEmailServiceProd;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices(services =>
            {

                services.AddLogging();
                services.AddApplicationInsightsTelemetryWorkerService();
                services.AddControllers();
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new() { Title = "SigOpsTools.API", Version = "v1" });
                });

                services.AddEndpointsApiExplorer();
                services.AddScoped<IIncidentRepository, CrashDataAccessLayer>();
                services.AddScoped<CrashController>();
            })
            .ConfigureAppConfiguration((context, config) =>
            {
            })
            .ConfigureFunctionsWebApplication()
            .Build();

        await host.RunAsync();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging();
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "SigOpsTools.API", Version = "v1" });
        });

        services.AddEndpointsApiExplorer();
        services.AddScoped<IIncidentRepository, CrashDataAccessLayer>();
        services.AddScoped<CrashController>();
    }
}