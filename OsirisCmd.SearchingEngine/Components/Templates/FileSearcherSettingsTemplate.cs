using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Lucene.Net.Util.Automaton;
using OsirisCmd.SettingsManager;

namespace OsirisCmd.SearchingEngine.Components.Templates;

public class FileSearcherSettingsTemplate : IDataTemplate
{
    public Control? Build(object? param)
    {
        var setting = param as SettingItem;

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
                        Text = string.Join(", ", (List<string>)setting.Value),
                        VerticalAlignment = VerticalAlignment.Center
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