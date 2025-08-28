using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<int>>
{
    private readonly IUserRepository _userRepository;

    private readonly ILogger<CreateUserCommandHandler> _logger;

    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        ILogger<CreateUserCommandHandler> logger,
        IUnitOfWork unitOfWork
    )
    {
        _userRepository = userRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var email = Domain.ValueObjects.Email.Create(request.Email);

            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                return Result<int>.Failure("User with this email already exists");

            var user = new Domain.Entities.User(
                email,
                request.FirstName,
                request.LastName
            );
            await _userRepository.AddAsync(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User created with ID: {UserId}", user.Id);

            return Result<int>.Success(user.Id);
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed for user creation");
            return Result<int>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return Result<int>.Failure("An error occurred while creating user");
        }
    }
}