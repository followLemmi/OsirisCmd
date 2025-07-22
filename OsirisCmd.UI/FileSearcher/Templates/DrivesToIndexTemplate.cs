using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using OsirisCmd.SearchingEngine.Settings;
using OsirisCmd.Services.Events;

namespace OsirisCmd.UI.FileSearcher.Templates;

public class DrivesToIndexTemplate : IDataTemplate
{
    public Control? Build(object? param)
    {
        var driveToIndex = param as DriveToIndex;
        driveToIndex!.PropertyChanged += (sender, args) => SettingChangedEvent.Invoke(driveToIndex);
        return new Grid()
        {
            ColumnDefinitions =
            {
                new ColumnDefinition() {Width = GridLength.Auto},
                new ColumnDefinition() {Width = GridLength.Star},
                new ColumnDefinition() {Width = GridLength.Auto}           
            },
            Children =
            {
                new TextBlock()
                {
                    Text = driveToIndex?.Name ?? "Unknown",
                    [Grid.ColumnProperty] = 0,
                },
                new ToggleSwitch()
                {
                    IsChecked = driveToIndex?.Enabled ?? false,
                    OffContent = null,
                    OnContent = null,
                    [!ToggleButton.IsCheckedProperty] = new Binding("Enabled", BindingMode.TwoWay),
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    [Grid.ColumnProperty] = 2    
                }
                
            }
        };
    }

    public bool Match(object? data)
    {
        return data is DriveToIndex;
    }
}