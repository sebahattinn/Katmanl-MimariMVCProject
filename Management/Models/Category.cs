namespace OnlineGallery.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; }

    // Navigation property
    public ICollection<Artwork> Artworks { get; set; }
}

