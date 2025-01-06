using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineGallery.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }
        public string? Address { get; set; }

        [Required(ErrorMessage = "Password Hash is required.")]
        public string PasswordHash { get; set; } // Şifre için Hash işlemi yapılacak

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; }  // Kullanıcının rolü
        public string? ProfileImageUrl { get; set; } = "default.jpg"; // Kullanıcının profil resmi

        // Navigation properties
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}