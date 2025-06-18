using System.ComponentModel;

namespace OsirisCmd.SettingsManager;

public class SettingItem : INotifyPropertyChanged
{
    public string Name { get; set; }
    private object _value;

    public object Value
    {
        get => _value;
        set
        {
            if (!(!_value?.Equals(value) ?? value != null)) return;
            _value = value;
            OnPropertyChanged(nameof(Value));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

}