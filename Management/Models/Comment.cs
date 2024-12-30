using OnlineGallery.Models;

public class Comment
{
    public int CommentId { get; set; }
    public int ArtworkId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; }
    public DateTime CommentDate { get; set; }

    // Navigation properties
    public Artwork Artwork { get; set; }
    public User User { get; set; }
}

