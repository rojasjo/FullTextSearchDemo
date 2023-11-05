using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.SearchEngine.Helpers;

internal static class DocumentFieldsHelper
{
    internal static IEnumerable<string> GetStringField<T>() where T : IDocument
    {
        var instance = Activator.CreateInstance<T>();

        // Search all string properties for the search term
        return typeof(T).GetProperties().Select(property => property.Name)
            .Select(fieldName => new { fieldName, type = instance.GetType().GetProperty(fieldName)?.PropertyType })
            .Where(p => p.fieldName != nameof(IDocument.UniqueKey))
            .Where(t => t.type != null)
            .Where(t => t.type == string.Empty.GetType())
            .Select(t => t.fieldName);
    }
}