using System.Reflection;
using FullTextSearchDemo.SearchEngine.Models;
using Lucene.Net.Documents;

namespace FullTextSearchDemo.SearchEngine.Helpers;

internal static class DocumentConverterExtensions
{
    /// <summary>
    /// Reconstructs a generic object from a Lucene document.
    /// Nested objects are not supported.
    /// </summary>
    /// <param name="document"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    internal static T ConvertToObjectOfType<T>(this Document document) where T : IDocument
    {
        var instance = Activator.CreateInstance<T>();

        foreach (var property in typeof(T).GetProperties())
        {
            var fieldName = property.Name;
            var field = document.GetField(fieldName);

            if (field == null)
            {
                continue;
            }

            var fieldValue = field.GetStringValue();
            SetPropertyValue(property, instance, fieldValue);
        }

        return instance;
    }

    /// <summary>
    /// Stores the properties of the generic object as text fields in a Lucene document.
    /// </summary>
    /// <param name="instance"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    internal static Document ConvertToDocument<T>(this T instance) where T : IDocument
    {
        var document = new Document();

        foreach (var property in typeof(T).GetProperties())
        {
            var value = property.GetValue(instance) ?? string.Empty;

            var fieldName = property.Name;

            var field = new TextField(fieldName, value.ToString(), Field.Store.YES);
            document.Add(field);
        }

        return document;
    }

    private static void SetPropertyValue<T>(PropertyInfo property, T instance, string fieldValue)
        where T : IDocument
    {
        var propertyType = property.PropertyType;

        if (propertyType == typeof(string) && property.Name != nameof(IDocument.UniqueKey))
        {
            property.SetValue(instance, fieldValue);
        }
        else if (propertyType == typeof(int))
        {
            property.SetValue(instance, int.Parse(fieldValue));
        }
        else if (propertyType == typeof(long))
        {
            property.SetValue(instance, long.Parse(fieldValue));
        }
        else if (propertyType == typeof(double))
        {
            property.SetValue(instance, double.Parse(fieldValue));
        }
        else if (propertyType == typeof(bool))
        {
            property.SetValue(instance, bool.Parse(fieldValue));
        }
        else if (propertyType == typeof(byte))
        {
            property.SetValue(instance, byte.Parse(fieldValue));
        }
        else if (propertyType == typeof(sbyte))
        {
            property.SetValue(instance, sbyte.Parse(fieldValue));
        }
        else if (propertyType == typeof(char))
        {
            property.SetValue(instance, char.Parse(fieldValue));
        }
        else if (propertyType == typeof(decimal))
        {
            property.SetValue(instance, decimal.Parse(fieldValue));
        }
        else if (propertyType == typeof(float))
        {
            property.SetValue(instance, float.Parse(fieldValue));
        }
        else if (propertyType == typeof(uint))
        {
            property.SetValue(instance, uint.Parse(fieldValue));
        }
        else if (propertyType == typeof(nint))
        {
            property.SetValue(instance, nint.Parse(fieldValue));
        }
        else if (propertyType == typeof(nuint))
        {
            property.SetValue(instance, nuint.Parse(fieldValue));
        }
        else if (propertyType == typeof(ulong))
        {
            property.SetValue(instance, ulong.Parse(fieldValue));
        }
        else if (propertyType == typeof(short))
        {
            property.SetValue(instance, short.Parse(fieldValue));
        }
        else if (propertyType == typeof(ushort))
        {
            property.SetValue(instance, ushort.Parse(fieldValue));
        }
    }
}