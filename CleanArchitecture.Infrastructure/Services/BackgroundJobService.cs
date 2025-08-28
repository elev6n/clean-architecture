using System.Linq.Expressions;
using CleanArchitecture.Application.Interfaces;
using Hangfire;

namespace CleanArchitecture.Infrastructure.Services;

public class BackgroundJobService : IBackgroundJobService
{
    public string Enqueue<T>(Expression<Action<T>> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }

    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(methodCall, delay);
    }

    public string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt)
    {
        return BackgroundJob.Schedule(methodCall, enqueueAt);
    }
}
