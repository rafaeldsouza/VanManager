using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using VanManager.Application.Common.Interfaces;

namespace VanManager.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        try
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
            var smtpUsername = emailSettings["SmtpUsername"];
            var smtpPassword = emailSettings["SmtpPassword"];
            var senderEmail = emailSettings["SenderEmail"];
            var senderName = emailSettings["SenderName"];

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail ?? string.Empty, senderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            // In a real application, we might want to handle this exception more gracefully
            // For development purposes, we'll just log the error
        }
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        var appUrl = _configuration["AppUrl"] ?? "https://vanmanager.com";
        var resetUrl = $"{appUrl}/resetar-senha?token={resetToken}&email={WebUtility.UrlEncode(email)}";
        
        var subject = "Redefinição de Senha - VanManager";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #4a6ee0;'>Redefinição de Senha</h2>
                    <p>Você solicitou a redefinição de sua senha no VanManager.</p>
                    <p>Clique no botão abaixo para criar uma nova senha:</p>
                    <p style='text-align: center;'>
                        <a href='{resetUrl}' style='display: inline-block; background-color: #4a6ee0; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                            Redefinir Senha
                        </a>
                    </p>
                    <p>Se você não solicitou esta redefinição, ignore este e-mail.</p>
                    <p>Este link expirará em 24 horas.</p>
                    <p>Atenciosamente,<br>Equipe VanManager</p>
                </div>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string email, string userName)
    {
        var appUrl = _configuration["AppUrl"] ?? "https://vanmanager.com";
        
        var subject = "Bem-vindo ao VanManager";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #4a6ee0;'>Bem-vindo ao VanManager</h2>
                    <p>Olá {userName},</p>
                    <p>Obrigado por se cadastrar no VanManager, a plataforma completa para gestão de transporte escolar.</p>
                    <p>Acesse agora mesmo e comece a usar:</p>
                    <p style='text-align: center;'>
                        <a href='{appUrl}/login' style='display: inline-block; background-color: #4a6ee0; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                            Acessar o VanManager
                        </a>
                    </p>
                    <p>Se você tiver alguma dúvida, entre em contato com nosso suporte.</p>
                    <p>Atenciosamente,<br>Equipe VanManager</p>
                </div>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body);
    }
}