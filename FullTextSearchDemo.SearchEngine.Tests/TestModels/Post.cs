namespace FullTextSearchDemo.SearchEngine.Tests.TestModels;

public class Post
{
    public long Id { get; set; }

    public required string Title { get; set; }

    public required string Content { get; set; }
}