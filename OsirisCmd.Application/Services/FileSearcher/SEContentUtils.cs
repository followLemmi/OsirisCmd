using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotNet.Globbing;

namespace Application;

public class SEContentUtils
{
    public static Dictionary<int, Dictionary<string, List<int>>> parseResultEntries(
        string filePath,
        string contentRequest,
        bool isCaseSensitive
    )
    {
        var lineNumber = 0;
        var entries = new Dictionary<int, Dictionary<string, List<int>>>();

        var isWildcardPattern = contentRequest.Contains('*') || contentRequest.Contains('?');
        var isPhraseSearchPattern =
            contentRequest.StartsWith('\"') && contentRequest.EndsWith('\"');
        var isMultiWordPhraseSearchPattern =
            contentRequest.Contains(' ') && !isWildcardPattern && !isPhraseSearchPattern;

        var comparisonType = isCaseSensitive
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

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
            else if (isWildcardPattern)
            {
                var globOption = new GlobOptions
                {
                    Evaluation = { CaseInsensitive = isCaseSensitive },
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
                var lineEntries = GetLineEntriesPositions(
                    line,
                    contentRequest,
                    isWildcardPattern,
                    isCaseSensitive
                );
                entries.Add(
                    lineNumber,
                    new Dictionary<string, List<int>> { { contentRequest, lineEntries } }
                );
            }

            lineNumber++;
        }

        return entries;
    }

    public static List<int> GetLineEntriesPositions(
        string line,
        string searchingText,
        bool isWildcardPattern,
        bool caseSensitive
    )
    {
        var result = new List<int>();
        if (isWildcardPattern)
        {
            string pattern = Regex.Escape(searchingText).Replace("\\*", ".*").Replace("\\?", ".");
            var regex = new Regex(pattern);

            foreach (Match match in regex.Matches(line))
            {
                result.Add(match.Index);
            }
        }
        else
        {
            var index = line.IndexOf(
                searchingText,
                caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase
            );
            while (index != -1)
            {
                result.Add(index);
                index = line.IndexOf(
                    searchingText,
                    index + searchingText.Length,
                    caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase
                );
            }
        }
        return result;
    }

    public static Dictionary<int, string> filterCaseSensitiveContent(
        Dictionary<int, string> entries,
        string contentRequest
    )
    {
        return (
            from line in entries
            let glob = Glob.Parse(contentRequest)
            where glob.IsMatch(line.Value)
            select line
        ).ToDictionary(line => line.Key, line => line.Value);
    }
}
