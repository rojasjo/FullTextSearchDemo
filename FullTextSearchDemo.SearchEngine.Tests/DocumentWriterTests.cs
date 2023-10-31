using System.Dynamic;
using FullTextSearchDemo.SearchEngine.Configuration;
using FullTextSearchDemo.SearchEngine.Services;

namespace FullTextSearchDemo.SearchEngine.Tests;

[TestFixture]
public class DocumentWriterTests
{
    private class WrongConfiguration : IIndexConfiguration<object>
    {
        public string IndexName => string.Empty;
    }

    [Test]
    public void Initialize_Always_ThrowsException()
    {
        dynamic dynamicObject = new ExpandoObject();
        dynamicObject.FirstName = "John";
        dynamicObject.LastName = "Doe";

        Assert.Throws<ArgumentException>(() => _ = new DocumentWriter<object>(new WrongConfiguration()));
    }
}