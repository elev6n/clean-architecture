using CleanArchitecture.Application.Users.Commands.CreateUser;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CleanArchitecture.Tests.Application.Users;

public class CreateUserCommandHandlerTests
{
    private readonly CreateUserCommandHandler _handler;

    private readonly Mock<IUserRepository> _userRepositoryMock;

    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    private readonly Mock<IBackgroundJobService> _backgroundJobServiceMock;

    public CreateUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _backgroundJobServiceMock = new Mock<IBackgroundJobService>();
        _handler = new CreateUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _backgroundJobServiceMock.Object,
            Mock.Of<ILogger<CreateUserCommandHandler>>()
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateUserCommand(
            "test@example.com",
            "John",
            "Doe"
        );

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<Email>()))
            .ReturnsAsync((Domain.Entities.User?)null);

        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Domain.Entities.User>()))
            .Callback<Domain.Entities.User>(user =>
            {
                typeof(Domain.Entities.User)
                    .GetProperty("Id")?
                    .SetValue(user, 1);
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Handle_ExistingEmail_ReturnsFailure()
    {
        // Arrange
        var command = new CreateUserCommand(
            "test@example.com",
            "John",
            "Doe"
        );

        var existingUser = new Domain.Entities.User(
            Email.Create("existing@example.com"), "John", "Doe");
        
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<Email>()))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("User with this email already exists");
    }
}
