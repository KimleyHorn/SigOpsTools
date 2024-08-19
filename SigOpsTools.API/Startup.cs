using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SigOpsTools.API;
using SigOpsTools.API.Models;

namespace SigOpsTools.API
{


    public class Startup
    {
        public Startup(IServiceCollection services)
        {
            ConfigureServices(services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new() { Title = "SigOpsTools.API", Version = "v1" }); });

            services.AddEndpointsApiExplorer();
            services.AddScoped<IIncidentRepository, CrashDataAccessLayer>();
        }

    }
}
