using Microsoft.Data.Sqlite;

namespace DBStorageAdapter;

public interface IDatabaseContext
{
    void Initialize();
    
    SqliteConnection GetConnection();
}