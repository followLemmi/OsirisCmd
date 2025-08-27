using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using OsirisCmd.Services.Events;

namespace OsirisCmd.UI.Application.FileSearcher;

public class SearchTemplate : IDataTemplate
{
    public Control? Build(object? param)
    {
        var searchType = param as SearchTypeViewModel;
        Debug.Assert(searchType != null, nameof(searchType) + " != null");
        return searchType.SearchType switch
        {
            SearchType.FileName or SearchType.FileContent => CreateBaseTemplate(),
            SearchType.FileContentAndFileName => CreateExtendedTemplate(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static Grid CreateBaseTemplate()
    {
        var grid = new Grid();
        
        var textBox = new TextBox();
        textBox.Watermark = "Type here to search...";
        textBox.TextChanged += (sender, args) =>
        {
            TextTypedOnSearchBoxEvent.Invoke(textBox.Text ?? string.Empty);
        };
        
        grid.Children.Add(textBox);
        return grid;
    }

    private static Grid CreateExtendedTemplate()
    {
        var grid = new Grid();
        grid.Background = new LinearGradientBrush()
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromRgb(250, 250, 250), 0),    // Светлый верх
                new GradientStop(Color.FromRgb(240, 240, 240), 0.5),  // Средний
                new GradientStop(Color.FromRgb(230, 230, 230), 1)     // Темный низ
            }
 
        };
        grid.RowDefinitions.Add(new RowDefinition() {Height = GridLength.Auto});
        grid.RowDefinitions.Add(new RowDefinition() {Height = GridLength.Auto});
        
        var textBox = new TextBox();
        textBox.Watermark = "Type here to search...";
        textBox.Margin = new Thickness(0, 10, 0, 10);
        textBox.TextChanged += (sender, args) =>
        {
            TextTypedOnSearchBoxEvent.Invoke(textBox.Text ?? string.Empty);
        };
        textBox[Grid.RowProperty] = 0;
        
        
        var textBox2 = new TextBox();
        textBox2.Watermark = "Type here to search...";
        textBox2.Margin = new Thickness(0, 10, 0, 10);
        textBox2.TextChanged += (sender, args) =>
        {
            TextTypedOnSearchBoxEvent.Invoke(textBox2.Text ?? string.Empty);
        };
        textBox2[Grid.RowProperty] = 1;
        
        grid.Children.Add(textBox);
        grid.Children.Add(textBox2);
        return grid;
    }

    public bool Match(object? data) => data is SearchTypeViewModel;
}