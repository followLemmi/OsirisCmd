using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Controls.Shapes;
using DotNet.Globbing;

namespace Application.Services.FileSearcher;

public class SEContentUtils
{
    public static Dictionary<int, List<LineMatchResult>> ParseResultEntries(string filePath,
        string contentRequest, bool isCaseSensitive)
    {
        var lineNumber = 0;
        var entries = new Dictionary<int, List<LineMatchResult>>();

        var isWildcardPattern = contentRequest.Contains('*') || contentRequest.Contains('?');
        var isPhraseSearchPattern = contentRequest.StartsWith('\"') && contentRequest.EndsWith('\"');
        var isMultiWordPhraseSearchPattern =
            contentRequest.Contains(' ') && !isWildcardPattern && !isPhraseSearchPattern;

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
                var lineEntries = GetLineEntriesPositions(line, contentRequest, isWildcardPattern, isCaseSensitive);
                entries.Add(lineNumber, lineEntries);
            }

            lineNumber++;
        }

        return entries;
    }

    public static List<LineMatchResult> GetLineEntriesPositions(string line, string searchingText, bool isWildcardPattern,
        bool caseSensitive)
    {
        var result = new List<LineMatchResult>();
        if (isWildcardPattern)
        {
            string targetWord = null;
            var isPrefix = false;
            var isSuffix = false;

            if (searchingText.StartsWith('*') && searchingText.EndsWith('*') && searchingText.Length > 2)
            {
                targetWord = searchingText.Substring(1, searchingText.Length - 2);
            }
            else if (searchingText.StartsWith('*') && !searchingText.EndsWith('*'))
            {
                targetWord = searchingText.Substring(1);
                isPrefix = true;
            }
            else if (!searchingText.StartsWith('*') && searchingText.EndsWith('*'))
            {
                targetWord = searchingText.Substring(0, searchingText.Length - 1);
                isSuffix = true;
            }

            if (!string.IsNullOrEmpty(targetWord))
            {
                var comparisonType = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                if (isPrefix || isSuffix)
                {
                    var words = Regex.Split(line, @"\b");
                    var currentPosition = 0;

                    foreach (var word in words)
                    {
                        var wordIndex = line.IndexOf(word, currentPosition, comparisonType);
                        if (wordIndex >= 0)
                        {
                            var matches = false;
                            if (isPrefix && word.StartsWith(targetWord, comparisonType))
                            {
                                matches = true;
                            }
                            else if (isSuffix && word.EndsWith(targetWord, comparisonType))
                            {
                                matches = true;
                            }

                            if (matches)
                            {
                                result.Add(new LineMatchResult
                                {
                                    StartIndex = wordIndex,
                                    EndIndex = wordIndex + word.Length,
                                    Text = word,
                                });
                            }

                            currentPosition = wordIndex + word.Length;
                        }
                    }
                }
                else
                {
                    var index = line.IndexOf(targetWord, comparisonType);
                    while (index != -1)
                    {
                        var word = line.Substring(index, targetWord.Length);
                        result.Add(new LineMatchResult
                        {
                            StartIndex = index,
                            EndIndex = index + targetWord.Length,
                            Text = word,
                        });
                        index = line.IndexOf(targetWord, index + targetWord.Length, comparisonType);
                    }
                }
            }
            else
            {
                var pattern = Regex.Escape(searchingText).Replace("\\*", ".*?").Replace("\\?", ".");
                var regex = new Regex(pattern, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);

                var currentIndex = 0;
                while (currentIndex < line.Length)
                {
                    var match = regex.Match(line, currentIndex);
                    if (!match.Success)
                    {
                        break;
                    }
                    result.Add(new LineMatchResult
                    {
                        StartIndex = match.Index,
                        EndIndex = match.Index + match.Length,
                        Text = match.Value,
                    });
                    currentIndex = match.Index + Math.Max(1, match.Length);
                }
            }
        }
        else
        {
            var index = line.IndexOf(searchingText, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
            while (index != -1)
            {
                var word = line.Substring(index, searchingText.Length);
                result.Add(new LineMatchResult
                {
                    StartIndex = index,
                    EndIndex = index + searchingText.Length,
                    Text = word,
                });
                index = line.IndexOf(searchingText, index + searchingText.Length,
                    caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
            }
        }

        return result;
    }
}