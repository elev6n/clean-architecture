namespace CleanArchitecture.Application.Auth;

public record LoginRequest(string Email, string Password);

public record AuthResponse(string Token, DateTime ExpiresAt);

public record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
);