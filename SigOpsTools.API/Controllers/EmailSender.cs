using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ConfigurationManager = System.Configuration.ConfigurationManager;

// Base class with default email sending behavior
public class EmailSender : IEmailSender
{
    protected readonly string smtpServer;
    protected readonly int smtpPort;
    protected readonly string smtpUser;
    protected readonly string smtpPass;

    public EmailSender(string smtpServer, int smtpPort, string smtpUser, string smtpPass)
    {
        this.smtpServer = smtpServer;
        this.smtpPort = smtpPort;
        this.smtpUser = smtpUser;
        this.smtpPass = smtpPass;
    }

    public virtual async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(smtpUser),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(new MailAddress(email));

        using var client = new SmtpClient(smtpServer, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        try
        {
            await client.SendMailAsync(mailMessage);
            Console.WriteLine("Email sent successfully.");
        }
        catch (SmtpException ex)
        {
            Console.WriteLine($"SMTP Exception: {ex.Message}");
            Console.WriteLine($"Status Code: {ex.StatusCode}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            throw;
        }
    }
}

// Derived class with specific sender and password
public class CustomEmailSender : EmailSender
{
    private const string defaultSender = "brandon.hall@kimley-horn.com";
    private const string defaultPassword = "";

    public CustomEmailSender(string smtpServer, int smtpPort)
        : base(smtpServer, smtpPort, defaultSender, defaultPassword)
    {
    }

    public override async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        await base.SendEmailAsync(email, subject, htmlMessage);
    }
}
