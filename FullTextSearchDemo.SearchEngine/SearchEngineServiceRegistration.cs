using FullTextSearchDemo.SearchEngine.Configuration;
using FullTextSearchDemo.SearchEngine.Engine;
using FullTextSearchDemo.SearchEngine.Models;
using FullTextSearchDemo.SearchEngine.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FullTextSearchDemo.SearchEngine;

public static class SearchEngineServiceRegistration
{
    /// <summary>
    /// Adds search engine services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of document to be used for search operations.</typeparam>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to which services are added.</param>
    /// <param name="configuration">The configuration for the search engine.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> with added search engine services.</returns>
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