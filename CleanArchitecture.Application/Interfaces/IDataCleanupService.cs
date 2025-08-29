namespace CleanArchitecture.Application.Interfaces;

public interface IDataCleanupService
{
    Task CleanupOldDataAsync();
}
