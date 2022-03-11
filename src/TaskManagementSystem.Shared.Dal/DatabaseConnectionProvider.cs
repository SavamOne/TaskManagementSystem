using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;
using TaskManagementSystem.Shared.Dal.Options;

namespace TaskManagementSystem.Shared.Dal;

public class DatabaseConnectionProvider : IDisposable
{
    private readonly IOptions<PostgresOptions> options;

    private IDbConnection? connection;
    private bool disposed;

    public DatabaseConnectionProvider(IOptions<PostgresOptions> options)
    {
        this.options = options;
    }

    public void Dispose()
    {
        connection?.Dispose();
        disposed = true;
    }

    public IDbConnection GetConnection()
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(DatabaseConnectionProvider));
        }

        if (connection?.State != ConnectionState.Open)
        {
            connection?.Dispose();
            connection = new NpgsqlConnection(options.Value.ConnectionString);
            connection.Open();
        }

        return connection;
    }
}