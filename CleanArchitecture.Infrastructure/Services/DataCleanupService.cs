using CleanArchitecture.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Services;

public class DataCleanupService : IDataCleanupService
{
    private readonly ILogger<DataCleanupService> _logger;

    public DataCleanupService(ILogger<DataCleanupService> logger)
    {
        _logger = logger;
    }

    public async Task CleanupOldDataAsync()
    {
        _logger.LogInformation("Starting data cleanup...");
        await Task.Delay(2000);
        _logger.LogInformation("Data cleanup completed");
    }
}
