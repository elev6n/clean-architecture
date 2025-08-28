namespace CleanArchitecture.Application.Interfaces;

public interface IEmailService
{
    Task SendWelcomeEmail(string email, string firstName);
    Task SendPasswordResetEmail(string email);
}
