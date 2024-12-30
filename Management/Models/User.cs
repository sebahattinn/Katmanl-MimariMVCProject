namespace OnlineGallery.Models;

public class User
{
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? Address { get; set; }
    public string PasswordHash { get; set; } // Şifre için Hash işlemi yapılacak
    public string Role { get; set; } = "user"; // Kullanıcının rolü

    // Navigation properties
    public ICollection<Sale> Sales { get; set; }
    public ICollection<Comment> Comments { get; set; }
}

