using CleanArchitecture.Domain.Exceptions;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Domain.Entities;

public class User : EntityBase
{
    public Email Email { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public DateOnly CreatedAt { get; private set; }

    public DateOnly? UpdatedAt { get; private set; }

    private User() { }

    public User(Email email, string firstName, string lastName)
    {
        if (string.IsNullOrEmpty(email))
            throw new DomainException("Email is required");

        Email = email;
        FirstName = firstName;
        LastName = lastName;
        CreatedAt = DateOnly.FromDateTime(DateTime.Now);
    }

    public void UpdateName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}