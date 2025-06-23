using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Lucene.Net.Util.Automaton;
using OsirisCmd.Localization;
using OsirisCmd.SearchingEngine.Converters;
using OsirisCmd.SettingsManager;
using OsirisCmd.SettingsManager.Events;

namespace OsirisCmd.SearchingEngine.Components.Templates;

public class FileSearcherSettingsTemplate : IDataTemplate
{
    public Control? Build(object? param)
    {
        var setting = param as SettingItem;
        setting!.PropertyChanged += (sender, args) => SettingChangedEvent.Invoke(setting);

        return setting.Value switch
        {
            bool _ => new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 5, 0, 5),
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = GridLength.Auto },
                    new ColumnDefinition() { Width = GridLength.Star },
                    new ColumnDefinition() {Width = GridLength.Auto}
                },
                Children =
                {
                    new TextBlock()
                    {
                        Text = LocalizationService.GetString(setting.Name) ?? setting.Name,
                        FontSize = 16,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        [Grid.ColumnProperty] = 0       
                    },
                    new CheckBox()
                    {
                        IsChecked = (bool)setting.Value,
                        [!ToggleButton.IsCheckedProperty] = new Binding("Value", BindingMode.TwoWay),
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        [Grid.ColumnProperty] = 2    
                    }
                }
            },
            List<string> _ => new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 5, 0, 5),
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = GridLength.Auto },   
                    new ColumnDefinition() { Width = GridLength.Star },
                    new ColumnDefinition() {Width = GridLength.Auto}
                },
                Children =
                {
                    new TextBlock()
                    {
                        Text = setting.Name,
                        VerticalAlignment = VerticalAlignment.Center,
                        [Grid.ColumnProperty] = 0
                    },
                    new TextBox()
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 400,
                        [!TextBox.TextProperty] = new Binding("Value")
                        {
                            Mode = BindingMode.TwoWay,
                            Converter = new StringListConverter(),
                            Source = setting
                        },
                        TextWrapping = TextWrapping.Wrap,
                        [Grid.ColumnProperty] = 2
                    }
                }
            },
            List<SettingItem> s => new Expander()
            {
                Header = setting.Name,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 5, 0, 5),
                Content = new ItemsControl()
                {
                    ItemsSource = s,
                    ItemTemplate = this,
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