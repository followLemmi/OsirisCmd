using Microsoft.Data.Sqlite;

namespace OsirisCmd.DBStorageAdapter;

public abstract class BaseDbModule : IDatabaseContext, IDisposable
{
    protected readonly SqliteConnection Connection;

    protected BaseDbModule(string dbPath)
    {
        Connection = new SqliteConnection($"Data Source={dbPath};Mode=ReadWriteCreate;");
        Connection.Open();
        Initialize();
    }

    public abstract void Initialize();

    public SqliteConnection GetConnection()
    {
        return Connection;
    }

    public void Dispose()
    {
        Connection.Dispose();
    }
}