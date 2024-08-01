using SendGrid;
using SendGrid.Helpers.Mail;
using SigOpsTools.API.Models;
using ConfigurationManager = System.Configuration.ConfigurationManager;

// Base class with default email sending behavior
namespace SigOpsTools.API.Controllers;

public class EmailSender
{
    readonly string apiKey = ConfigurationManager.AppSettings["SENDGRID_API_KEY"];
    readonly string fromEmail = ConfigurationManager.AppSettings["SMTP_USER"] ?? "brandon.hall@kimley-horn.com";


    public EmailSender()
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentNullException("SENDGRID_API_KEY", "API key must not be null or empty.");
        }
    }


    public async Task<bool> SendGridEmailAsync(string recipient, Incident i)
    {
        var htmlContent = CreateEmail(i);
        string fromName = "Crash Reports";
        //TODO Pass this in a different way
        //string toEmail = "thomas.glueckert@kimley-horn.com";
        string subject = "New Crash Reported";
        string plainTextContent = "This is a test email sent using SendGrid with plain text.";
        try
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(recipient);
            plainTextContent = CreateEmail(i);
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
    public string CreateEmail(Incident i)
    {
        //if (i.EventType != "accidentsAndIncident")
        //    return "";
        var message = i.Description + $"\n Reported at: {i.DateReported} in {i.Region}";
        return message;
    }


}