using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OsirisCmd.core.PluginManager;

namespace OsirisCmd.Core.SettingsStorage;

public class PluginSettings : INotifyPropertyChanged
{
    private string _name;
    private bool _enabled;

    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled != value)
            {
                _enabled = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}