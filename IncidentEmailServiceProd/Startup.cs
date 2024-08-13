using IncidentEmailServiceProd;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace IncidentEmailServiceProd
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
            var startup = new Startup();
            startup.ConfigureServices(services);
        }
    }
}
