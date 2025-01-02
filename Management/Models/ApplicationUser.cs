using Microsoft.AspNetCore.Identity;  // Identity sınıflarını kullanabilmek için gerekli

namespace OnlineGallery.Models  // Kendi Models namespace'inizle eşleştirin
{
    // IdentityUser sınıfından türemek zorunlu
    public class ApplicationUser : IdentityUser
    {
        // Burada ek olarak kullanıcıya ait bilgiler tutabilirsiniz.
        // Örnek: Kullanıcı adı, profil resmi vs.

        public string FullName { get; set; }  // Kullanıcının tam adı
        public string Email { get; set; }
        public string Role { get; set; }         //burdan sonrasını ekledm
    }
}
