using System;
using System.Collections.Generic;

namespace Application.Services.FileSearcher;

public class LineMatchResult
{
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public string Text { get; set; }

    private sealed class StartIndexEndIndexTextEqualityComparer : IEqualityComparer<LineMatchResult>
    {
        public bool Equals(LineMatchResult? x, LineMatchResult? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.StartIndex == y.StartIndex && x.EndIndex == y.EndIndex && x.Text == y.Text;
        }

        public int GetHashCode(LineMatchResult obj)
        {
            return HashCode.Combine(obj.StartIndex, obj.EndIndex, obj.Text);
        }
    }

    public static IEqualityComparer<LineMatchResult> StartIndexEndIndexTextComparer { get; } = new StartIndexEndIndexTextEqualityComparer();
}