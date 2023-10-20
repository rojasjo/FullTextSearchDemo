namespace FullTextSearchDemo.Models;

public class Product
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    
    public float Price { get; set; }
}