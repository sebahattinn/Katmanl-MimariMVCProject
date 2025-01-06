using Microsoft.AspNetCore.Identity;  // Identity sınıflarını kullanabilmek için gerekli

namespace OnlineGallery.Models  
{
    // IdentityUser sınıfından türemek zorunlu
    public class ApplicationUser : IdentityUser
    {
      

        public string FullName { get; set; }  // Kullanıcının tam adı
        public string Email { get; set; }
        public string Role { get; set; }         //burdan sonrasını ekledm
    }
}
