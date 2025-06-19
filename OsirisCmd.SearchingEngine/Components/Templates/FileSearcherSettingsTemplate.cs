using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Lucene.Net.Util.Automaton;
using OsirisCmd.SearchingEngine.Converters;
using OsirisCmd.SettingsManager;
using SettingsManager.Events;

namespace OsirisCmd.SearchingEngine.Components.Templates;

public class FileSearcherSettingsTemplate : IDataTemplate
{
    public Control? Build(object? param)
    {
        var setting = param as SettingItem;
        setting!.PropertyChanged += (sender, args) => SettingChangedEvent.Invoke();

        return setting.Value switch
        {
            bool _ => new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new TextBlock()
                    {
                        Text = setting.Name,
                        Width = 200,
                        VerticalAlignment = VerticalAlignment.Center
                    },
                    new CheckBox()
                    {
                        IsChecked = (bool)setting.Value,
                        [!ToggleButton.IsCheckedProperty] = new Binding("Value", BindingMode.TwoWay),
                        VerticalAlignment = VerticalAlignment.Center
                    }
                }
            },
            List<string> _ => new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new TextBlock()
                    {
                        Text = setting.Name,
                        Width = 200,
                        VerticalAlignment = VerticalAlignment.Center
                    },
                    new TextBox()
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        [!TextBox.TextProperty] = new Binding("Value")
                        {
                            Mode = BindingMode.TwoWay,
                            Converter = new StringListConverter(),
                            Source = setting
                        }
                    }
                }
            },
            _ => new TextBlock()
            {
                Text = $"Unsupported type: {setting.Value.GetType()} of {setting.Name}"
            }
        };
    }

    public bool Match(object? data) => data is SettingItem;
}