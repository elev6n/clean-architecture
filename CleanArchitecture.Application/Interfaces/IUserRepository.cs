using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;

namespace CleanArchitecture.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);

    Task<User?> GetByEmailAsync(Email email);

    Task AddAsync(User user);

    void UpdateAsync(User user);

    Task<bool> ExistsAsync(Email email);
}
