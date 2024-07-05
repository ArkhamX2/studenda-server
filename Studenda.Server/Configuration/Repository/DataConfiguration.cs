using Microsoft.EntityFrameworkCore;
using Studenda.Server.Data.Configuration;

namespace Studenda.Server.Configuration.Repository;

public class DataConfiguration(IConfiguration configuration) : ConfigurationRepository(configuration)
{
    private const string SectionName = "Data";

    private string GetConnectionType()
    {
        var result = Configuration
            .GetSection(SectionName)
            .GetValue<string>("ConnectionType");

        return HandleStringValue(result, "Connection type is null or empty!");
    }

    public ContextConfiguration GetDefaultContextConfiguration(bool isDebugMode)
    {
        var connectionString = Configuration.GetConnectionString("Default");

        HandleStringValue(connectionString, "Default connection string is null or empty!");

        return GetConnectionType().ToLower() switch
        {
            "sqlite" => new SqliteConfiguration(connectionString!, isDebugMode),
            "mysql" => new MysqlConfiguration(connectionString!, ServerVersion.AutoDetect(connectionString),
                isDebugMode),
            _ => throw new Exception("Unknown connection type!")
        };
    }
}