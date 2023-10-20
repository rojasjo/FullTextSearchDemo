using FullTextSearchDemo.SearchEngine.Configuration;
using FullTextSearchDemo.SearchEngine.Engine;
using FullTextSearchDemo.SearchEngine.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FullTextSearchDemo.SearchEngine;

public static class SearchEngineServiceRegistration
{
    public static IServiceCollection AddSearchEngineServices<T>(this IServiceCollection serviceCollection, IIndexConfiguration<T> configuration) where T : class
    {
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddScoped<IDocumentFactory<T>, DocumentFactory<T>>();
        serviceCollection.AddScoped<ISearchEngine<T>, SearchEngine<T>>();

        return serviceCollection;
    }
}