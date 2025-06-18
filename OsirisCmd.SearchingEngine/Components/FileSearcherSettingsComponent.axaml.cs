using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OsirisCmd.SearchingEngine.ViewModels;

namespace OsirisCmd.SearchingEngine.Components;

public partial class FileSearcherSettingsComponent : UserControl
{
    public FileSearcherSettingsComponent()
    {
        InitializeComponent();
        DataContext = new FileSearcherSettingsViewModel();
    }
}