namespace OnlineGallery.Models;

public class Artwork
{
    public int ArtworkId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public required string ImageUrl { get; set; }

    // Navigation property
    public required Category Category { get; set; }
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}