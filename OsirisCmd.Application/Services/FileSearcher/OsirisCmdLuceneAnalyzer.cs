using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;

namespace Application;

public class OsirisCmdLuceneAnalyzer : Analyzer
{
    protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
    {
        var tokinizer = new WhitespaceTokenizer(Lucene.Net.Util.LuceneVersion.LUCENE_48, reader);
        TokenStream tokenStream = tokinizer;
        return new TokenStreamComponents(tokinizer, tokenStream);
    }
}
