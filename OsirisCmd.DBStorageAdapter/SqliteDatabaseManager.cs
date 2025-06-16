namespace OsirisCmd.DBStorageAdapter;

public class SqliteDatabaseManager(string dbPath) : IDisposable
{
    private readonly List<BaseDbModule> _dbModules = [];

    public T GetModule<T>() where T : BaseDbModule
    {
        foreach (var module in _dbModules.OfType<T>())
        {
            return module;
        }
        var newModule = (T)Activator.CreateInstance(typeof(T), dbPath)!;
        _dbModules.Add(newModule);
        return newModule;
    }

    public void Dispose()
    {
        foreach (var module in _dbModules)
        {
            module.Dispose();
        }
    }
}