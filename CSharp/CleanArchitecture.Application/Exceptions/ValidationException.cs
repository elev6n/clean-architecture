namespace CleanArchitecture.Application.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(IEnumerable<ValidationError> errors)
        : base("Validation failed") => Errors = errors;

    public IEnumerable<ValidationError> Errors { get; }
}

public record ValidationError(
    string PropertyName,
    string ErrorMessage
);
