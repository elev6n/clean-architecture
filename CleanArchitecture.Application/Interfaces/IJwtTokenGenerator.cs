namespace CleanArchitecture.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(int userId, string email, IEnumerable<string> roles);

    int? ValidateToken(string token);
}
