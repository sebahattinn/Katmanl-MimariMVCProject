using Microsoft.AspNetCore.Mvc;
using OnlineGallery.Data;
using OnlineGallery.Models; // Artwork modelini kullanmak i�in eklenmeli
using System.Linq;

namespace OnlineGallery.Controllers
{
    public class HomeController : Controller
    {
        private readonly GalleryDbContext _context;

        public HomeController(GalleryDbContext context)
        {
            _context = context;
        }

        // Ana sayfa
        public IActionResult Index()
        {
            // Artworks verilerini veritaban�ndan al�yoruz
            var artworks = _context.Artworks.ToList();

            // Verileri View'a g�nderiyoruz
            return View(artworks);
        }


        // Artworks listesini partial view'da g�stermek i�in action
        public IActionResult GetArtworkList()
        {
            // Artworks verilerini al�yoruz
            var artworks = _context.Artworks.ToList();

            // Partial view'a veriyi g�nderiyoruz
            return PartialView("_ArtworkList", artworks);
        }
        //burdan sonras� eklendi
        public IActionResult Blog()
        {
            return View("Blog");
        }

        public IActionResult WhyUs()
        {
            return View("WhyUs");
        }

        public IActionResult About()
        {
            return View("About");
        }

        public IActionResult Contact()
        {
            return View("Contact");
        }
        //Buraya Kadar
    }
}
