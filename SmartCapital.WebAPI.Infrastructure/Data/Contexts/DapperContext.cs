using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace SmartCapital.WebAPI.Infrastructure.Data.Contexts;
public class DapperContext : IDisposable
{
    private readonly string _connectionString;
    private IDbConnection? _connection = null;
    private bool _disposedValue;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration["ConnectionStrings:SmartCapitalDatabase"] ?? throw new InvalidOperationException("A string de conexão não esta definida.");
    }

    public IDbConnection CreateConnection()
    {
        _connection = new MySqlConnection(_connectionString);

        return _connection;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _connection?.Dispose();
            }

            _disposedValue = true;
        }
    }

    void IDisposable.Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
