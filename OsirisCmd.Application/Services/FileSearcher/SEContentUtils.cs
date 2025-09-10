using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNet.Globbing;

namespace Application;

public class SEContentUtils
{
    public static Dictionary<int, string> parseResultEntries(string filePath, string contentRequest,
        bool isCaseSensitive)
    {
        var lineNumber = 0;
        var entries = new Dictionary<int, string>();

        var isGlobPattern = contentRequest.Contains('*') || contentRequest.Contains('?');
        var isPhraseSearchPattern = contentRequest.StartsWith('\"') && contentRequest.EndsWith('\"');
        var isMultiWordPhraseSearchPattern = contentRequest.Contains(' ') && !isGlobPattern && !isPhraseSearchPattern;

        var comparisonType = isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        foreach (var line in File.ReadLines(filePath))
        {
            var matches = false;
            if (isPhraseSearchPattern)
            {
                var phrase = contentRequest.Substring(1, contentRequest.Length - 2);
                matches = line.Contains(phrase, comparisonType);
            }
            else if (isMultiWordPhraseSearchPattern)
            {
                var words = contentRequest.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                matches = words.All(word => line.Contains(word, comparisonType));
            }
            else if (isGlobPattern)
            {
                var globOption = new GlobOptions
                {
                    Evaluation =
                    {
                        CaseInsensitive = isCaseSensitive
                    }
                };
                var glob = Glob.Parse(contentRequest, globOption);
                matches = glob.IsMatch(line);
            }
            else
            {
                matches = line.Contains(contentRequest, comparisonType);
            }

            if (matches)
            {
                entries.Add(lineNumber, line);
            }

            lineNumber++;
        }

        return entries;
    }

    public static Dictionary<int, string> filterCaseSensitiveContent(Dictionary<int, string> entries,
        string contentRequest)
    {
        return (from line in entries let glob = Glob.Parse(contentRequest) where glob.IsMatch(line.Value) select line)
            .ToDictionary(line => line.Key, line => line.Value);
    }
}