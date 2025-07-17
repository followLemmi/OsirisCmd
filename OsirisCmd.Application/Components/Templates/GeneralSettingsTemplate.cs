using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using OsirisCmd.SettingsManager;

namespace Application.Components.Templates;

public class GeneralSettingsTemplate : IDataTemplate
{
    public Control? Build(object? param)
    {
        var setting = (SettingItem)param!;
        return setting.Value switch
        {
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
            }
        };
    }

    public bool Match(object? data) => data is SettingItem;
}