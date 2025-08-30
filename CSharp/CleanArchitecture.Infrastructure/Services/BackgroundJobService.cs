using System.Linq.Expressions;
using CleanArchitecture.Application.Interfaces;

namespace CleanArchitecture.Infrastructure.Services;
public class BackgroundJobService : IBackgroundJobService
{
    public string Enqueue(Expression<Action> methodCall)
    {
        return Hangfire.BackgroundJob.Enqueue(methodCall);
    }

    public string Enqueue<T>(Expression<Action<T>> methodCall)
    {
        return Hangfire.BackgroundJob.Enqueue(methodCall);
    }

    public string Schedule(Expression<Action> methodCall, TimeSpan delay)
    {
        return Hangfire.BackgroundJob.Schedule(methodCall, delay);
    }

    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
    {
        return Hangfire.BackgroundJob.Schedule(methodCall, delay);
    }

    public string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt)
    {
        return Hangfire.BackgroundJob.Schedule(methodCall, enqueueAt);
    }

    public string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt)
    {
        return Hangfire.BackgroundJob.Schedule(methodCall, enqueueAt);
    }

    public string AddOrUpdate(string recurringJobId, Expression<Action> methodCall, string cronExpression)
    {
        Hangfire.RecurringJob.AddOrUpdate(recurringJobId, methodCall, cronExpression);
        return recurringJobId;
    }

    public string AddOrUpdate<T>(string recurringJobId, Expression<Action<T>> methodCall, string cronExpression)
    {
        Hangfire.RecurringJob.AddOrUpdate(recurringJobId, methodCall, cronExpression);
        return recurringJobId;
    }

    public bool Delete(string jobId)
    {
        return Hangfire.BackgroundJob.Delete(jobId);
    }
}