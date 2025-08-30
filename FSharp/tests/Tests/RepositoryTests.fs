namespace RepositoryTests

open System
open System.Threading.Tasks
open Xunit
open Microsoft.EntityFrameworkCore

module TestHelpers =
    open CleanArchitecture.Infrastructure

    let createInMemoryDbContext () =
        let options =
            DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options

        new AppDbContext(options)

module UserRepositoryTests =
    open CleanArchitecture.Infrastructure.Repositories
    open CleanArchitecture.Domain.RepositoryInterfaces
    open CleanArchitecture.Domain.User
    open CleanArchitecture.Domain.Common

    [<Fact>]
    let ``UserRepository Create should return user with generated Id`` () =
        async {
            // Arrange
            use dbContext = TestHelpers.createInMemoryDbContext ()
            let repository = UserRepository dbContext :> IUserRepository

            let request: CreateUserRequest =
                { Email = Email "test@example.com"
                  Username = Username "testuser" }

            // Act
            let! result = repository.Create request

            // Assert
            match result.Id with
            | guid -> Assert.NotEqual(EntityId Guid.Empty, guid)

            Assert.Equal(request.Email, result.Email)
            Assert.Equal(request.Username, result.Username)
            Assert.True(result.CreatedAt <= DateTime.UtcNow)
            Assert.True(result.UpdatedAt <= DateTime.UtcNow)
        }
        |> Async.StartAsTask
        :> Task
