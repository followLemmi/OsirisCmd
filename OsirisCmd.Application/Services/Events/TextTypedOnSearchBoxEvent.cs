namespace Application.Services.Events;

public class TextTypedOnSearchBoxEvent
{
    private delegate void TextTypedOnSearchBoxDelegate(string text);
    
    private static event TextTypedOnSearchBoxDelegate? TextTypedOnSearchBox;
    
    public static void Invoke(string text)
    {
        TextTypedOnSearchBox!.Invoke(text);
    }
}