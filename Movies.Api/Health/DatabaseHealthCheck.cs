using Microsoft.Extensions.Diagnostics.HealthChecks;
using Movies.Application.Database;

namespace Movies.Api.Health;

public class DatabaseHealthCheck(IDbConnectionFactory dbConnectionFactory, ILogger<DatabaseHealthCheck> logger) : IHealthCheck
{
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken token = default)
    {
        try
        {
            await dbConnectionFactory.CreateConnectionAsync(token);
        }
        catch (Exception e)
        {
            const string errorMessage = "Database is unhealthy";
            logger.LogError(errorMessage);
            return HealthCheckResult.Unhealthy(errorMessage, e);
        }

        return HealthCheckResult.Healthy();
    }
}