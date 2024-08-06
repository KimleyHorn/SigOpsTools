using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SigOpsTools.API;
using SigOpsTools.API.Models;

public class Startup
{
    public Startup(IServiceCollection services)
    {
        ConfigureServices(services);
    }
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Configure token validation parameters
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("MyPolicy", policy => { policy.RequireAuthenticatedUser(); });
        });

        services.AddControllers();
        services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new() { Title = "SigOpsTools.API", Version = "v1" }); });

        services.AddEndpointsApiExplorer();
        services.AddScoped<IIncidentRepository, CrashDataAccessLayer>();
    }

}