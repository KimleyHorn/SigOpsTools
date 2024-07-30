using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SigOpsTools.API;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using CrashEmailService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SigOpsTools.API", Version = "v1" });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IIncidentRepository, CrashDataAccessLayer>();
builder.Services.AddTransient<IEmailSender>(serviceProvider =>
{
    var _smtpServer = ConfigurationManager.AppSettings["SmtpServer"] ?? string.Empty;
    var _smtpPort = int.Parse(ConfigurationManager.AppSettings["smtpPort"] ?? "587");
    var _smtpUser = ConfigurationManager.AppSettings["smtpUser"] ?? string.Empty;
    var  _smtpPass = ConfigurationManager.AppSettings["smtpPass"] ?? string.Empty;
    return new EmailSender(_smtpServer, _smtpPort, _smtpUser, _smtpPass);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
