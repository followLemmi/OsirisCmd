using Avalonia.Controls;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace Application.UI.FileSearcher;

public partial class FilePreviewWindow : Window
{
    
    public FilePreviewWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => LoadWindowStyles();
    }

    private void LoadWindowStyles()
    {
        if (DataContext is FilePreviewWindowViewModel viewModel)
        {
            var textEditor = FilePreview;
            var registryOption = new RegistryOptions(ThemeName.DarkPlus);
            var textMateInstallation = textEditor.InstallTextMate(registryOption);
            textMateInstallation.SetGrammar(registryOption.GetScopeByLanguageId(registryOption.GetLanguageByExtension(viewModel.Extension).Id));
        }
    }
}