namespace OnlineGallery.Models;

public class Artwork
{
    public int ArtworkId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string ImageUrl { get; set; }

    // Navigation property
    public Category Category { get; set; }
    public ICollection<Sale> Sales { get; set; }
    public ICollection<Comment> Comments { get; set; }
}

