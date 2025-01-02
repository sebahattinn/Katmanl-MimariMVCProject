using Microsoft.AspNetCore.Mvc;
using OnlineGallery.Data;
using OnlineGallery.Models;
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

        // Main page
        public IActionResult Index()
        {
            // Fetch artworks from the database
            var artworks = _context.Artworks.ToList();

            // Send data to the view
            return View(artworks);
        }

        // Action to show artworks list in a partial view
        public IActionResult GetArtworkList()
        {
            // Fetch artworks
            var artworks = _context.Artworks.ToList();

            // Send data to the partial view
            return PartialView("_ArtworkList", artworks);
        }

        // Additional actions
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
    }
}