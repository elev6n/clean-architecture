using CleanArchitecture.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendWelcomeEmail(string email, string firstName)
    {
        // Имитация отправки email
        _logger.LogInformation("Sending welcome email to {Email}", email);
        await Task.Delay(1000); // Имитация работы
        _logger.LogInformation("Welcome email sent to {Email}", email);
    }

    public async Task SendPasswordResetEmail(string email)
    {
        _logger.LogInformation("Sending password reset email to {Email}", email);
        await Task.Delay(1000);
        _logger.LogInformation("Password reset email sent to {Email}", email);
    }
}