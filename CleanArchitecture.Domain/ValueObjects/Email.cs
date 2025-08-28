using CleanArchitecture.Domain.Exceptions;

namespace CleanArchitecture.Domain.ValueObjects;

public record Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty");

        if (!email.Contains('@'))
            throw new DomainException("Email is invalid");

        return new Email(email.ToLower());
    }

    public static implicit operator string(Email email) => email.Value;

    public override string ToString() => Value;
}
