namespace OnlineGallery.Models;

public class Sale
{
    public int SaleId { get; set; }
    public int ArtworkId { get; set; }
    public int UserId { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal Price { get; set; }

    // Navigation properties
    public Artwork Artwork { get; set; }
    public User User { get; set; }
}
