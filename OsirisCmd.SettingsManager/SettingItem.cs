using System.ComponentModel;
using System.Text.Json.Serialization;
using OsirisCmd.SettingsManager.Converters;

namespace OsirisCmd.SettingsManager;

[JsonConverter(typeof(SettingsItemConverter))]
public class SettingItem : INotifyPropertyChanged
{
    public string Name { get; set; }
    public string Type { get; set; } = string.Empty;
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

    public override string ToString() => $"SettingItem {Name}: Value = {Value}, Type = ({Type})";
}