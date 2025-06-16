using OsirisCmd.DBStorageAdapter;

namespace OsirisCmd.FileSystem;

public class FileSystemStorageDbModule(string dbPath) : BaseDbModule(dbPath)
{
    public override void Initialize()
    {
        var cmd = Connection.CreateCommand();
        
        
    }
}