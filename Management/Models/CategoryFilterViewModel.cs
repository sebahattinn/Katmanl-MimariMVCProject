namespace OnlineGallery.Models
{
    public class CategoryFilterViewModel
    {
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Artwork> Artworks { get; set; } = new List<Artwork>();

    }
}
