using System.Linq.Expressions;

namespace CleanArchitecture.Application.Interfaces;

public interface IBackgroundJobService
{
    string Enqueue<T>(Expression<Action<T>> methodCall);

    string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);

    string Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt);
}
