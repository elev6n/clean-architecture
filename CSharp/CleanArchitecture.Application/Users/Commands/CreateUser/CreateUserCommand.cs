using CleanArchitecture.Application.Common.Models;
using MediatR;

namespace CleanArchitecture.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Email,
    string FirstName,
    string LastName
) : IRequest<Result<int>>;
