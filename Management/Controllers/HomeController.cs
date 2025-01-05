using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;
using System;
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

        // Action to show artwork details
        public IActionResult TableDetails(int id)
        {
            var artwork = _context.Artworks
                .Include(a => a.Comments)  // Comments'leri de dahil ediyoruz
                .ThenInclude(c => c.User)  // Eðer yorum yapan kullanýcý bilgileri de gerekiyorsa
                .FirstOrDefault(a => a.ArtworkId == id);

            if (artwork == null)
            {
                return NotFound();
            }

            return View(artwork);
        }

        // Action to show the purchase page for the artwork
        public IActionResult SatinAl(int id)
        {
            var artwork = _context.Artworks.FirstOrDefault(a => a.ArtworkId == id);
            if (artwork == null)
            {
                return NotFound();
            }

            return View(artwork); // Render the purchase page
        }

        // Post action for completing the purchase
        [HttpPost]
        public IActionResult CompletePurchase(int artworkId, decimal price, string creditCard, string billingAddress)
        {
            var userId = int.Parse(User.Identity.Name); // Assuming the user is authenticated and userId is stored in the identity

            // Create a new Sale record
            var sale = new Sale
            {
                ArtworkId = artworkId,
                UserId = userId,
                SaleDate = DateTime.Now,
                Price = price
            };

            // Save the sale record to the database
            _context.Sales.Add(sale);
            _context.SaveChanges();

            // After saving the sale, you can redirect to a confirmation or thank you page
            return RedirectToAction("PurchaseSuccess", new { id = artworkId });
        }

        // Action for purchase success page
        public IActionResult PurchaseSuccess(int id)
        {
            var artwork = _context.Artworks.FirstOrDefault(a => a.ArtworkId == id);
            if (artwork == null)
            {
                return NotFound();
            }

            return View(artwork); // Show a confirmation page after purchase
        }


        // Action to add a comment
        [HttpPost]
        public IActionResult AddComment(int artworkId, string content, string userId)
        {
            var comment = new Comment
            {
                ArtworkId = artworkId,
                UserId = int.Parse(userId),
                Content = content,
                CommentDate = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("TableDetails", new { id = artworkId });
        }


        // Action to show artworks list in a partial view
        public IActionResult GetArtworkList()
        {
            // Fetch artworks
            var artworks = _context.Artworks.ToList();

            // Send data to the partial view
            return PartialView("_ArtworkList", artworks);
        }

        // Blog page
        public IActionResult Blog()
        {
            return View("Blog");
        }

        // WhyUs page
        public IActionResult WhyUs()
        {
            return View("WhyUs");
        }

        // About page
        public IActionResult About()
        {
            return View("About");
        }

        // Contact page
        public IActionResult Contact()
        {
            return View("Contact");
        }

        // Action to handle payment with iyzipay
        public IActionResult IyzicoPayment(int artworkId, decimal price)
        {
            var paymentModel = new PaymentViewModel
            {
                ArtworkId = artworkId,
                Price = price,
                Token = "a5b67652-c24a-4347-b61e-6c957bf30f1b",  // Sample Token (Replace with dynamic value)
                PaymentPageUrl = "https://sandbox-cpp.iyzipay.com?token=a5b67652-c24a-4347-b61e-6c957bf30f1b&lang=en" // Redirect to payment page
            };

            return View("IyzicoPayment", paymentModel);
        }

        // Action to handle payment success
        [HttpPost]
        public IActionResult CompleteIyzicoPayment(int artworkId, decimal price, string paymentStatus)
        {
            if (paymentStatus == "success")
            {
                var userId = int.Parse(User.Identity.Name); // Assuming the user is authenticated and userId is stored in the identity

                // Create a new Sale record after successful payment
                var sale = new Sale
                {
                    ArtworkId = artworkId,
                    UserId = userId,
                    SaleDate = DateTime.Now,
                    Price = price
                };

                // Save the sale record to the database
                _context.Sales.Add(sale);
                _context.SaveChanges();

                // Redirect to the success page
                return RedirectToAction("PurchaseSuccess", new { id = artworkId });
            }

            // If payment failed, show failure page
            return RedirectToAction("PurchaseFailed");
        }

        // Action to show payment failure page
        public IActionResult PurchaseFailed()
        {
            return View("PurchaseFailed");
        }

        // Search artworks by category
        public IActionResult SearchByCategory(int categoryId)
        {
            // Seçilen kategoriye ait sanat eserlerini al
            var artworks = _context.Artworks
                                   .Where(a => a.CategoryId == categoryId)
                                   .ToList();
            return View("Index", artworks);
        }
    }
}
