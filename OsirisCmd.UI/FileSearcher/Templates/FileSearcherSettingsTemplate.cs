using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using OsirisCmd.Core.Models;
using OsirisCmd.SearchingEngine.Converters;
using OsirisCmd.SearchingEngine.Settings;
using OsirisCmd.Services.Events;

namespace OsirisCmd.UI.FileSearcher.Templates;

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
                    new ColumnDefinition() { Width = GridLength.Auto }
                },
                Children =
                {
                    new TextBlock()
                    {
                        Text = setting.Name,
                        FontSize = 16,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        [Grid.ColumnProperty] = 0
                    },
                    new ToggleSwitch()
                    {
                        IsChecked = (bool)setting.Value,
                        OffContent = null,
                        OnContent = null,
                        [!ToggleButton.IsCheckedProperty] = new Binding("Value", BindingMode.TwoWay),
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        [Grid.ColumnProperty] = 2
                    }
                }
            },
            string _ => new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 5, 0, 5),
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = GridLength.Auto },
                    new ColumnDefinition() { Width = GridLength.Star },
                    new ColumnDefinition() { Width = GridLength.Auto }
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
                        },
                        TextWrapping = TextWrapping.Wrap,
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
                    new ColumnDefinition() { Width = GridLength.Auto }
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
                IsExpanded = true,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 5, 0, 5),
                Content = new ItemsControl()
                {
                    ItemsSource = s,
                    ItemTemplate = this,
                }
            },
            List<DriveToIndex> s => new Expander()
            {
                Header = "Drives to index",
                IsExpanded = true,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 5, 0, 5),
                Content = new ItemsControl()
                {
                    ItemsSource = s,
                    ItemTemplate = new DrivesToIndexTemplate(),
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