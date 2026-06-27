using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DomusNet.API.Services;

public class EmailService : Interfaces.IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpHost = _config["Email:SmtpHost"];
        var smtpPortStr = _config["Email:SmtpPort"];
        var senderEmail = _config["Email:SenderEmail"];
        var senderPassword = _config["Email:SenderPassword"];
        var enableSslStr = _config["Email:EnableSsl"] ?? "true";

        if (string.IsNullOrWhiteSpace(to))
        {
            _logger.LogWarning("No se pudo enviar correo porque el destinatario está vacío.");
            return;
        }

        // Si faltan parámetros, se simula el envío para pruebas locales
        if (string.IsNullOrWhiteSpace(smtpHost) ||
            string.IsNullOrWhiteSpace(senderEmail) ||
            string.IsNullOrWhiteSpace(senderPassword))
        {
            _logger.LogInformation(
                "\n============================================================\n" +
                "              [SIMULACIÓN DE ENVÍO DE CORREO]\n" +
                "============================================================\n" +
                "De: DomusNet Soporte <soporte@domusnet.com>\n" +
                "Para: {To}\n" +
                "Asunto: {Subject}\n" +
                "Cuerpo:\n{Body}\n" +
                "============================================================\n",
                to,
                subject,
                body
            );

            await Task.Delay(100);
            return;
        }

        int smtpPort = int.TryParse(smtpPortStr, out var port) ? port : 587;
        bool enableSsl = bool.TryParse(enableSslStr, out var ssl) ? ssl : true;

        try
        {
            using var mail = new MailMessage();

            mail.From = new MailAddress(senderEmail, "Soporte DomusNet");
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = false;

            // Para que las tildes y ñ se vean bien
            mail.SubjectEncoding = Encoding.UTF8;
            mail.BodyEncoding = Encoding.UTF8;

            using var smtp = new SmtpClient(smtpHost, smtpPort);

            smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtp.EnableSsl = enableSsl;

            _logger.LogInformation("Enviando correo a {To} vía SMTP {SmtpHost}.", to, smtpHost);

            await smtp.SendMailAsync(mail);

            _logger.LogInformation("Correo enviado exitosamente a {To}.", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar correo a {To}. Mensaje: {Message}", to, ex.Message);
        }
    }
}