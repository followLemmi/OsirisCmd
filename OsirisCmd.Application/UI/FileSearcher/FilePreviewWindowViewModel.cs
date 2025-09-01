using AvaloniaEdit.Document;

namespace Application.UI.FileSearcher;

public class FilePreviewWindowViewModel : ViewModelBase
{
    private TextDocument _fileContentDocument = new();
    
    public string FileName { get; set; }
    public string Extension { get; set; }

    public TextDocument FileContentDocument
    {
        get => _fileContentDocument;
        set => SetProperty(ref _fileContentDocument, value);
    }

    public string? FileContent
    {
        get => _fileContentDocument.Text;
        set 
        {
            if (value != null)
            {
                _fileContentDocument.Text = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FileContentDocument));
            }
        }
    }





public FilePreviewWindowViewModel()
{
}
}