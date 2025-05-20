using System.Net;
using System.Net.Mail;
using ApiRest_LabWebApp.Services;
using Microsoft.Extensions.Configuration;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendTemporaryPasswordEmailAsync(string toEmail, string temporaryPassword)
    {
        var smtpHost = _config["Email:SmtpHost"];
        var smtpPort = int.Parse(_config["Email:SmtpPort"]);
        var smtpUser = _config["Email:Username"];
        var smtpPass = _config["Email:Password"];
        var fromEmail = _config["Email:From"];

        var message = new MailMessage(fromEmail, toEmail)
        {
            Subject = "Acceso temporal a LabWebApp",
            Body = $"Tu contraseña temporal es: {temporaryPassword}\n\nDebe cambiarla al iniciar sesión.",
            IsBodyHtml = false
        };

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }
}
