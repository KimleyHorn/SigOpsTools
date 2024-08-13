﻿using System.Reflection;
using IncidentEmailServiceProd.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using ConfigurationManager = System.Configuration.ConfigurationManager;

// Base class with default email sending behavior
namespace IncidentEmailServiceProd.Controllers;

public class EmailSender
{
    private readonly string _apiKey = ConfigurationManager.AppSettings["SENDGRID_API_KEY"] ?? string.Empty;
    private readonly string _fromEmail = ConfigurationManager.AppSettings["SMTP_USER"] ?? "traction@kimley-horn.com";


    public EmailSender()
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new ArgumentNullException("SENDGRID_API_KEY", "API key must not be null or empty.");
        }
    }


    public async Task<bool> SendGridEmailAsync((string,string) recipient, Incident i)
    {
        var totalContent = CreateEmail(i, recipient.Item1);
        var htmlContent = totalContent.Item2;
        var fromName = "Incident Report";
        //TODO Pass this in a different way
        var subject = $"New Incident Report at {i.roadway_name}";
        try
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, fromName);
            var to = new EmailAddress(recipient.Item2, recipient.Item1);
            var plainTextContent = totalContent.Item1;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            Console.WriteLine($"Status Code: {response.StatusCode}");
            Console.WriteLine($"Headers: {response.Headers}");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Email sent successfully.");
                return true;
            }


        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"ArgumentNullException: {ex.Message}");
            // Handle missing API key or other null arguments
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HttpRequestException: {ex.Message}");
            // Handle network-related errors
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"InvalidOperationException: {ex.Message}");
            // Handle invalid operations, such as incorrect client setup
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            // Handle all other exceptions
        }            
        return false;
    }

    //TODO add a priority check to email in the future
    //TODO Add map feature to email in the future
    public (string htmlMessage, string plainMessage) CreateEmail(Incident i, string recipient)
    {
        var template = string.Empty;

        var binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        var projectPath = Directory.GetParent(binPath).Parent.Parent.FullName;

        // Combine the base path with the relative path to the Models folder
        var templatePath = Path.Combine(projectPath, "Models", "EmailTemplate.html");

        try
        {
            template = File.ReadAllText(templatePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Email not created " + ex);
        }

        var htmlMessage = template
            .Replace("{Recipient}", System.Net.WebUtility.HtmlEncode(recipient))
            .Replace("{RoadwayName}", System.Net.WebUtility.HtmlEncode(i.roadway_name))
            .Replace("{EventType}", System.Net.WebUtility.HtmlEncode(i.event_type))
            .Replace("{ID}", System.Net.WebUtility.HtmlEncode(i.ID))
            .Replace("{Latitude}", i.latitude.ToString())
            .Replace("{Longitude}", i.longitude.ToString())
            .Replace("{LanesAffected}", System.Net.WebUtility.HtmlEncode(i.lanes_affected))
            .Replace("{DateReported}", i.reported.ToString("f"))
            .Replace("{PlannedEndDate}", i.planned_end.ToString("f"))
            .Replace("{LastUpdated}", i.last_updated.ToString("f"));

        var plainMessage = CreatePlainTextEmail(i, recipient);

        return (htmlMessage, plainMessage);
    }



    public string CreatePlainTextEmail(Incident i, string recipient)
    {
        return $"To: {recipient}</br>" +
               $"Type: {i.roadway_name}, {i.event_type}, {i.ID}</br>" +
               $"Location: ({i.latitude}, {i.longitude}) on {i.roadway_name}</br>" +
               $"Closure Reason: {i.event_type}</br>" +
               $"Lanes Blocked: {i.lanes_affected}</br>" +
               $"Reported at: {i.reported:f}</br>" +
               $"Estimated Time of Clearance: {i.planned_end:f}</br>" +
               $"Last Updated: {i.last_updated:f}</br>";
    }


}