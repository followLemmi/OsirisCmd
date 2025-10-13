using System;
using Application.Services.FileSearcher;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace Application.Services.Utils;

public class FileSearcherUtils
{
    public static Query CreateQuery(string query, string field)
    {
        var userInput = query.Trim();
        var userInputWords = userInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

        if (userInput.EndsWith('~'))
        {
            var fuzzyInput = userInput.Substring(0, userInput.Length - 1);
            var parts = fuzzyInput.Split('~');
            if (parts.Length == 2 && int.TryParse(parts[1], out var editDistance))
            {
                return new FuzzyQuery(new Term(field, parts[0]), editDistance);
            }
            return new FuzzyQuery(new Term(field, fuzzyInput), 1);
        }

        if (!(userInputWords.Length > 1) && (userInput.Contains('*') || userInput.Contains('?')))
        {
            return new WildcardQuery(new Term(field, userInput.ToLower()));
        }

        if (userInputWords.Length > 1)
        {
            var phraseQuery = new PhraseQuery();
            foreach (var word in userInputWords)
            {
                var wordClone = word;
                if (wordClone.Contains('*') || wordClone.Contains('?') || wordClone.Contains('"'))
                {
                    wordClone = wordClone.Trim('*');
                    wordClone = wordClone.Trim('?');
                    wordClone = wordClone.Trim('"');
                }
                phraseQuery.Add(new Term(field, wordClone.ToLower()));
                phraseQuery.Slop = 0;
            }
            return phraseQuery;
        }
        
        return new WildcardQuery(new Term(field, $"*{userInput.ToLower()}*"));
    }
}