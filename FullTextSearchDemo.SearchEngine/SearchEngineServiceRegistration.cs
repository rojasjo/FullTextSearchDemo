using FullTextSearchDemo.SearchEngine.Configuration;
using FullTextSearchDemo.SearchEngine.Engine;
using FullTextSearchDemo.SearchEngine.Models;
using FullTextSearchDemo.SearchEngine.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FullTextSearchDemo.SearchEngine;

public static class SearchEngineServiceRegistration
{
    public static IServiceCollection AddSearchEngineServices<T>(this IServiceCollection serviceCollection,
        IIndexConfiguration<T> configuration) where T : IDocument
    {
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddSingleton<IDocumentWriter<T>, DocumentWriter<T>>();
        serviceCollection.AddScoped<IDocumentReader<T>, DocumentReader<T>>();
        serviceCollection.AddScoped<ISearchEngine<T>, SearchEngine<T>>();

        return serviceCollection;
    }
}