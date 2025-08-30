using CleanArchitecture.Application.Common.Behaviors;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Users.Queries.GetUserByIdQuery;

public record GetUserByIdQuery(int Id) : IRequest<Result<UserDto>>, ICacheable
{
    public string CacheKey => $"user:{Id}";

    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}
