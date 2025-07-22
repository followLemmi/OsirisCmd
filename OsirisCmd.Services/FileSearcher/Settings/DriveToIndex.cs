using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OsirisCmd.SearchingEngine.Settings;

public class DriveToIndex : INotifyPropertyChanged
{
    public string Name { get; set; }
    
    private bool _enabled;
    
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled == value) return;
            _enabled = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}