namespace CleanArchitecture.Domain.Interfaces;

public interface IDataCleanupService
{
    Task CleanupOldDataAsync();
}
