using System.Dynamic;
using FullTextSearchDemo.SearchEngine.Configuration;
using FullTextSearchDemo.SearchEngine.Services;
using FullTextSearchDemo.SearchEngine.Tests.TestModels;

namespace FullTextSearchDemo.SearchEngine.Tests;

[TestFixture]
public class DocumentWriterTests
{
    private class WrongConfiguration : IIndexConfiguration<Element>
    {
        public string IndexName => string.Empty;
        
        public FacetConfiguration<Element>? FacetConfiguration { get; }
    }

    [Test]
    public void Initialize_Always_ThrowsException()
    {
        dynamic dynamicObject = new ExpandoObject();
        dynamicObject.FirstName = "John";
        dynamicObject.LastName = "Doe";

        Assert.Throws<ArgumentException>(() => _ = new DocumentWriter<Element>(new WrongConfiguration()));
    }
}