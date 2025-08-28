namespace CleanArchitecture.Domain.Entities;

public class User
{
    public int Id { get; private set; }

    public string Email { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public DateTime CreatedAt { get; private set; }
}