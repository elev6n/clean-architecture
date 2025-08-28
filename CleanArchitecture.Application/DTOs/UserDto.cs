using System.Text.Json.Serialization;

namespace CleanArchitecture.Application.DTOs;

public class UserDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = null!;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = null!;

    [JsonPropertyName("createdAt")]
    public DateOnly CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateOnly? UpdatedAt { get; set; }
}