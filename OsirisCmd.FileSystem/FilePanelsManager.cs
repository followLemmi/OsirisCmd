namespace OsirisCmd.FileSystem;

public class FilePanelsManager
{
    private Dictionary<string, FilePanel> FilePanels = new();

    public FilePanelsManager()
    {
        LoadFilePanels();
    }
    
    private void LoadFilePanels()
    {
    }

    public Dictionary<string, FilePanel> GetFilePanels()
    {
        return FilePanels;
    }
    
    public string CreateNewFilePanel()
    {
        var filePanel = new FilePanel();
        var id = Guid.NewGuid().ToString();
        FilePanels.Add(id, filePanel);
        return id;
    }
}