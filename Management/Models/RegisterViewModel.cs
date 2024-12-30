namespace OnlineGallery.Models;

public class RegisterViewModel
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } = "user";
}
