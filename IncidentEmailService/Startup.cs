using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SigOpsTools.API; // Replace with your existing project's namespace

[assembly: FunctionsStartup(typeof(MyFunctionApp.Startup))]

namespace MyFunctionApp
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
            var startup = new SigOpsTools.API;
            startup.ConfigureServices(services);
        }
    }
}
