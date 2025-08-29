using System.Linq.Expressions;

namespace CleanArchitecture.Domain.Interfaces;

public interface IBackgroundJobService
{
    string Enqueue(Expression<Action> methodCall);

    string Enqueue<T>(Expression<Action<T>> methodCall);

    string Schedule(Expression<Action> methodCall, TimeSpan delay);

    string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);

    string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt);

    string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt);

    string AddOrUpdate(string recurringJobId, Expression<Action> methodCall, string cronExpression);

    string AddOrUpdate<T>(string recurringJobId, Expression<Action<T>> methodCall, string cronExpression);
    
    bool Delete(string jobId);
}