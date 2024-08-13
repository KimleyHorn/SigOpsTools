using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(IncidentEmailService.Startup))]

namespace IncidentEmailService
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;
            ConfigureServices(services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Use the service configuration method from the existing project
            var startup = new SigOpsTools.API.Startup(services);
            startup.ConfigureServices(services);
        }
    }
}
