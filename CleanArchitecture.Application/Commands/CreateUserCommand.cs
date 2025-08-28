using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Commands;

public record CreateUserCommand(
    string Email,
    string FirstName,
    string LastName
) : IRequest<UserDto>;
