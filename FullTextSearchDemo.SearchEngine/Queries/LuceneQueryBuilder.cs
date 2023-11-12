using FullTextSearchDemo.SearchEngine.Helpers;
using FullTextSearchDemo.SearchEngine.Models;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace FullTextSearchDemo.SearchEngine.Queries;

internal static class LuceneQueryBuilder
{
    internal static Query ConstructQuery<T>(IDictionary<string, string?>? searchFields, SearchType searchType)
        where T : IDocument
    {
        if (searchFields == null || searchFields.Count == 0 ||
            searchFields.All(p => string.IsNullOrWhiteSpace(p.Value)))
        {
            return new MatchAllDocsQuery();
        }

        var query = new BooleanQuery();
        var instance = Activator.CreateInstance<T>();
        foreach (var (fieldName, value) in searchFields)
        {   
            var type = instance.GetType().GetProperty(fieldName)?.PropertyType;

            if (type == null || (type != typeof(string) && type != typeof(string[])))
            {
                continue;
            }

            Query searchQuery = searchType switch
            {
                SearchType.ExactMatch => new TermQuery(new Term(fieldName, value)),
                SearchType.PrefixMatch => new PrefixQuery(new Term(fieldName, value)),
                SearchType.FuzzyMatch => new FuzzyQuery(new Term(fieldName, value)),
                _ => new TermQuery(new Term(fieldName, value))
            };

            query.Add(searchQuery, Occur.SHOULD);
        }

        return query;
    }

    internal static BooleanQuery ConstructFulltextSearchQuery<T>(FullTextSearchQuery searchQuery) where T : IDocument
    {
        var fields = DocumentFieldsHelper.GetStringField<T>();

        var query = new BooleanQuery();
        foreach (var field in fields)
        {
            var fuzzyQuery = new FuzzyQuery(new Term(field, searchQuery.SearchTerm));
            var wildcardQuery = new WildcardQuery(new Term(field, $"{searchQuery.SearchTerm}*"));

            query.Add(fuzzyQuery, Occur.SHOULD);
            query.Add(wildcardQuery, Occur.SHOULD);
        }

        return query;
    }
}