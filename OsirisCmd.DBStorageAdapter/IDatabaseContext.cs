using Microsoft.Data.Sqlite;

namespace OsirisCmd.DBStorageAdapter;

public interface IDatabaseContext
{
    void Initialize();
    
    SqliteConnection GetConnection();
}