using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data; // Veritabanı bağlamı için
using OnlineGallery.Models; // Artwork modeli için

namespace OnlineGallery.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly GalleryDbContext _context;

        public CategoriesController(GalleryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult SearchByCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                // Kategori belirtilmezse tüm eserleri getir
                return View(_context.Artworks.Include(a => a.Category).ToList());
            }

            var filteredArtworks = _context.Artworks
                .Include(a => a.Category) // Category ilişkisini yükle
                .Where(a => a.Category.Name.Contains(category, StringComparison.OrdinalIgnoreCase)) // Kategori ismine göre filtrele
                .ToList();

            return View(filteredArtworks);
        }

    }
}
