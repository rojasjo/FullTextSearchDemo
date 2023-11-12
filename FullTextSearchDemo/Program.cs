using FullTextSearchDemo.Search;
using FullTextSearchDemo.SearchEngine;
using FullTextSearchDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddSearchEngineServices(new ProductConfiguration());
builder.Services.AddSearchEngineServices(new MoviesConfiguration());

builder.Services.AddHostedService<MovieImporterService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();