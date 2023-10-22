# Lucene.NET powered search library for ASP.NET Core

## Discover the Apache Lucene search library and its rewrite for .NET. We'll use this knowledge to build a flexible and reusable library to simplify the implementation of the full-text search feature in your .NET applications.

### Introduction
Every kind software product has a fundamental requirement: enabling users to find what they are looking for. Whether it's a product in a e-commerce platform, an invoice in a payment system, a customer in the Customer Relationship Management (CRM) tool, or a post in a blog website, users expect an effective search functionality.

Even if technically you could use the ```LIKE``` operator in your ```SQL SELECT``` statement, it's usually inefficient in term of performance and limited in finding the relevant elements. For develop advanced search functionality it's recommended to use appropriate software designed specifically to handle such scenario.

In this context, one of the more popular and powerful library is Apache Lucene.

### Apache Lucene
Lucene Core is an opensource search library written in Java and it is well known and widely used thanks to the variety of features for performing searches and its outstanding performance. It provides an API to perform structured searches, full-text search, faceting, highlighting, query suggestion and more.


Many search engine are built on top of Apache Lucene for instance Apache Solr and ElasticSearch. Those software are used in productive environments empowering thousand of hundreds of concurrent users to search and find their data.

The Apache Lucene Core library has many rewrites in other programming languages other than Java: among them there is an interesting implementation in .NET.

### Lucene.NET

Lucene.NET is a complete rewrite of the Java library in order to facilitate the integration of such technology in the .NET applications.
The Apache Lucene.NET project aims to maintain the existing line-by-line port to C# by automating the porting process and synchronising the release schedule with the original library Apache Lucene Core library. It ensures expected performance while taking advantage of the features of the .NET runtime.
Before getting our hands dirty in the code I'd like to give a bit of more context on some concepts that we need to keep in my while working with this library:
Indexing: an index is used to store and manage the data you want to search. Indexing is the process of creating this data structure that enables efficient retrieval. It involves breaking down your documents into tokens and storing them in a structured format in a specific folder.
 Documents: documents are the units of data you want to search, and they consist of one or more fields.
Analysers: analysers are responsible for processing text data from documents during indexing and searching. They handle tasks like tokenisation and text normalisation to ensure consistency in the index and search operations.
Querying: querying is the process of searching the index to retrieve documents that match specific criteria.

### Implementing the Search Library

To meet our requirement it is need to create a separate project in order to keep the library code separate from the one of the example asp.net web application.
The library has to hide to the user the complexity of dealing with the Lucene library directly. The ideal use case would ne that the developer register the SearchEngine and its related services and then inject it to perform searches in a easy and flexible way.

#### The ```DocumentWriter<T>```
At the core of the Search Library there is the ```DocumentWriter<T>```, this class is a ```singleton``` and for every type it will keep the reference to the corresponding Lucene.NET ```IndexWriter```. This will allow to perform the initialisation just on time, at the startup of the application, after that every request will be able to update the index and their updates will be seens by reader in near real-time.

```
internal sealed class DocumentWriter<T> : IDocumentWriter<T> where T : class
{
    private static readonly Lazy<DocumentWriter<T>> DocumentWriterInstance = new(() => new DocumentWriter<T>());

    public static DocumentWriter<T> Instance => DocumentWriterInstance.Value;

    /// <summary>
    /// The index name must be set before we can use the document writer.
    /// This static property is not shared between instances of the document writer for different types.
    /// </summary>
    public static string? Index { get; set; }

    public IndexWriter Writer { get; }

    private DocumentWriter()
    {
        if (string.IsNullOrWhiteSpace(Index))
        {
            throw new ArgumentException("Index name must be set before using DocumentWriter.");
        }

        // Open the index directory
        var indexPath = Path.Combine(Environment.CurrentDirectory, Index);
        LuceneDirectory indexDir = FSDirectory.Open(indexPath);

        // Create an analyzer to process the text
        const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;
        Analyzer standardAnalyzer = new StandardAnalyzer(luceneVersion);
        var indexConfig = new IndexWriterConfig(luceneVersion, standardAnalyzer)
        {
            OpenMode = OpenMode.CREATE_OR_APPEND
        };

        // Create the index writer with the above configuration
        Writer = new IndexWriter(indexDir, indexConfig);
    }
    
    public void WriteDocument(T generic)
    {
        var document = generic.ConvertToDocument();
        WriteDocument(document);
    }
    
    private void WriteDocument(Document document)
    {
        Writer.AddDocument(document);
        Writer.Commit();
    }
}

```

As you might noticed the ```Dependency Container``` would not be able to construct an instance of such ```class``` since its constructor is ```private``` because its ```singleton``` implementation.
A factory will allow us to create correctly the ```DocumentWriter<T>``` instance and return it to the client.

```
internal class DocumentFactory<T> : IDocumentFactory<T> where T : class
{
   public DocumentFactory(IIndexConfiguration<T> configuration)
   {
     DocumentWriter<T>.Index = configuration.IndexName;
   }

   public IDocumentReader CreateDocumentReader()
   {
     return new DocumentReader(DocumentWriter<T>.Instance.Writer.GetReader(true));
   }

   public IDocumentWriter<T> CreateDocumentWriter()
   {
     return DocumentWriter<T>.Instance;
   }
}
```

As you can see the ```IIndexConfiguration<T>``` interface allows the library user to set its ```IndexName```, which is the name of the index folder.

#### The ```DocumentReader<T>```

The ```DocumentReader<T>``` is the component responsible to get the user search criteria and construct a proper Lucene.NET query to perform the search and finally return the results to the callee.

```
internal sealed class DocumentReader : IDocumentReader
{
  private readonly IndexSearcher _searcher;

  public DocumentReader(IndexReader directoryReader)
  {
    _searcher = new IndexSearcher(directoryReader);
  }

  public IEnumerable<T> Search<T>(FieldSpecificSearchQuery searchQuery) where T : class
  {
    var query = ConstructQuery<T>(searchQuery.SearchTerms, searchQuery.Type)
    return PerformSearch<T>(query, searchQuery.PageNumber, searchQuery.PageSize);
  }

  public IEnumerable<T> Search<T>(AllFieldsSearchQuery searchQuery) where T : class
  {
    var instance = Activator.CreateInstance<T>()
 
    // Search all string properties for the search term
    var searchDictionary = typeof(T).GetProperties().Select(property => property.Name)
     .Select(fieldName => new { fieldName, type = instance.GetType().GetProperty(fieldName)?.PropertyType })
     .Where(t => t.type != null)
     .Where(t => t.type == string.Empty.GetType())
     .Select(t => t.fieldName).ToDictionary(fieldName => fieldName, fieldName => searchQuery.SearchTerm);

     var query = ConstructQuery<T>(searchDictionary, searchQuery.Type);
     return PerformSearch<T>(query, searchQuery.PageNumber, searchQuery.PageSize); 
  }

  private IEnumerable<T> PerformSearch<T>(Query query, int pageNumber, int pageSize) where T : class
  {
    var scoredDocs = _searcher.Search(query, int.MaxValue).ScoreDocs;var start = pageNumber * pageSize;
    var end = Math.Min(start + pageSize, scoredDocs.Length);

    if (start > end)
    {
       return Enumerable.Empty<T>();
    }
 
    return scoredDocs[start..end].Select(hit => _searcher.Doc(hit.Doc)).Select(d => d.ConvertToObjectOfType<T>())
     .ToList();
   }

  private static Query ConstructQuery<T>(IDictionary<string, string>? searchFiles, SearchType searchType)
   where T : class
   {
 
    if (searchFiles == null || searchFiles.Count == 0)
    {
      return new MatchAllDocsQuery();
    }

    if (searchFiles.All(p => string.IsNullOrWhiteSpace(p.Value)))
    {
      return new MatchAllDocsQuery();
    }

    var query = new BooleanQuery();
    var instance = Activator.CreateInstance<T>();
    foreach (var (fieldName, value) in searchFiles)
    {
       var type = instance.GetType().GetProperty(fieldName)?.PropertyType;

       if (type == null || type != typeof(string))
       {
         continue;
       }

       Query? searchQuery = searchType switch
       {
         SearchType.ExactMatch => new TermQuery(new Term(fieldName, value)),
         SearchType.PrefixMatch => new PrefixQuery(new Term(fieldName, value)),
         SearchType.FuzzyMatch => new FuzzyQuery(new Term(fieldName, value)),
         _ => null
       };

       query.Add(searchQuery, Occur.SHOULD);
     }

    return query;
  }
}

```

The ```DocumentWriter<T>```, ```DocumentReader<T>``` and ```DocumentFactory<T>``` are the core of the search library, their implementation are internal to ensure that its behaviour remains largely unaffected by the external users.

#### The Public API

Every library needs an public API that allow developers to use it in a convenient way.
Ideally the developer should be able to give enough search freedom to its users as much as possible.

The library will expose the following components:

* ```ISearchEngine<T>:``` its the instance that will be responsible interact with the core components already discussed.

* ```FieldSpecificSearchQuery```: enable the library users to configure the query by specifying the fields and the values to search for as well as the search type (exact match or fuzzy search) together with the pagination information.
* ```AllFieldsSearchQuery```: allow to configure the same search term for all properties of its ```Type```, along with the same configurations as the ```FieldSpecificSearchQuery```.
* ```SearchType```: it permits the configuration of how the search shuold be performed. It is possible to choose one of the three values available: ExactMatch, FuzzyMatch, and PrefixMatch.
* ```IIndexConfiguration<T>```: this public interface will be used to set the index name for a given ```Type```.
* ```AddSearchEngineServices<T>```: by calling this method for a custom ```Type```the library will register all the required services to the ```Dependency Container```.

### Example Asp.Net Core Application

The example application is a simple ASP.NET Core Web API project that references the search library that allow the users to add and search products. Mind that this project won't use a database since it is out the scope of this guide.

#### SearchEngine configuration and service registration

First of all, it is necessary to implement the ```IIndexConfiguration<T>```in order to be able to register the search engine for the ```Product``` entity.

```
public class ProductConfiguration : IIndexConfiguration<Product>
{
    public string IndexName => "product-index";
}

```

In the ```Program``` class we will register the search engines by simply calling the ```AddSearchEngineServices``` extension method.

```
...
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddSearchEngineServices(new ProductConfiguration());

var app = builder.Build();
...

```
#### ProductController

The ```Controller``` will use two classes to expose the search parameters to the client and handler better the requests:

* ```GetProductsQuery```: beside the pagination parameters, it exposes the fields to search a value in a specificy field e.g. the Name should be Pizza or the Description be Mozzarella and tomato.
* ```ProductsSearchQuery```: along with the pagination parameters, it exposes only a field for the search value.
 
The resulting implemention of the controller looks like:

```
[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public IActionResult GetProducts([FromQuery] GetProductsQuery query)
    {
        try
        {
            var result = _productService.GetProducts(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet("search")]
    public IActionResult SearchProducts([FromQuery] ProductsSearchQuery query)
    {
        try
        {
            var result = _productService.SearchProducts(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpPost]
    public IActionResult PostProduct([FromBody] Product product)
    {
        try
        {
            _productService.Add(product);
            return Ok("Product added to the search index.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}
```

#### The ```ProductService```

At this point we can have a look at the ProductService and how it is simple to use the ```ISearchEngine<T>```to perform searches.

```
public class ProductService : IProductService
{
    private readonly ISearchEngine<Product> _searchEngine;

    public ProductService(ISearchEngine<Product> searchEngine)
    {
        _searchEngine = searchEngine;
    }

    public IEnumerable<Product> GetProducts(GetProductsQuery query)
    {
        var searchTerm = new Dictionary<string, string>();
        
        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            searchTerm.Add(nameof(Product.Name), query.Name.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(query.Description))
        {
            searchTerm.Add(nameof(Product.Description), query.Description.ToLower());
        }

        var searchQuery = new FieldSpecificSearchQuery
        {
            SearchTerms = searchTerm,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            Type = SearchType.ExactMatch
        };

        return _searchEngine.Search(searchQuery);
    }

    public IEnumerable<Product> SearchProducts(ProductsSearchQuery query)
    {
        var searchQuery = new AllFieldsSearchQuery
        {
            SearchTerm = query.Search,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            Type = SearchType.FuzzyMatch
        };

        return _searchEngine.Search(searchQuery);
    }

    public void Add(Product product)
    {
        _searchEngine.Add(product);
    }
}

```

### Conclusion

In implementing this library, one of the key challenges I encountered was minimizing the overhead of data manipulation before harnessing the full power of Lucene.NET. While I used reflection for generic indexing and searches, it's worth noting that the impact on overall performance is generally negligible, as it primarily involves straightforward reflection operations.
It's important to keep in mind that the library doesn't support the storage of nested complex types.

Exposing all the features Lucene.NET offers can be a more involved process, potentially requiring some design adjustments. Nonetheless, in its current form, this initial version of the library provides the fundamental functionality needed to enhance search capabilities across various personal projects. Moreover, it has the potential to assist other developers in similar endeavors.

In summary, if you're in search of a production-ready solution, you may want to explore some of the popular and reliable search engines available in the market. I'd recommend considering Apache Solr, known for its simple and fast RESTful API. Ultimately, the choice of search technology should align with the specific needs and scale of your projects, and the journey to improved search functionality begins with the knowledge and tools at your disposal.
