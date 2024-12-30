using Microsoft.AspNetCore.Mvc;
using OnlineGallery.Data;
using OnlineGallery.Models; // Artwork modelini kullanmak için eklenmeli
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
            // Artworks verilerini veritabanýndan alýyoruz
            var artworks = _context.Artworks.ToList();

            // Verileri View'a gönderiyoruz
            return View(artworks);
        }


        // Artworks listesini partial view'da göstermek için action
        public IActionResult GetArtworkList()
        {
            // Artworks verilerini alýyoruz
            var artworks = _context.Artworks.ToList();

            // Partial view'a veriyi gönderiyoruz
            return PartialView("_ArtworkList", artworks);
        }
        //burdan sonrasý eklendi
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
