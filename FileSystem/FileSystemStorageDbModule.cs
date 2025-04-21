using DBStorageAdapter;

namespace FileSystem;

public class FileSystemStorageDbModule(string dbPath) : BaseDbModule(dbPath)
{
    public override void Initialize()
    {
        var cmd = Connection.CreateCommand();
        
        
    }
}