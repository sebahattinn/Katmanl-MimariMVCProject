using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Authorize attribute için gerekli
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece "Admin" rolüne sahip kullanıcılar erişebilir
    public class AdminController : Controller
    {
        private readonly GalleryDbContext _context;

        public AdminController(GalleryDbContext context)
        {
            _context = context;
        }

        // Kullanıcıları Listele
        public IActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        // Sanat Eseri Ekle
        public IActionResult AddArtwork()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddArtwork(Artwork artwork)
        {
            if (ModelState.IsValid)
            {
                _context.Artworks.Add(artwork);
                _context.SaveChanges();
                return RedirectToAction("Users");
            }
            return View();
        }
    }
}
