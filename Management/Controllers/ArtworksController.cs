using Microsoft.AspNetCore.Mvc;
using OnlineGallery.Models;
using System.Linq;
using OnlineGallery.Data;

public class ArtworksController : Controller
{
    private readonly GalleryDbContext _context; 

    public ArtworksController(GalleryDbContext context) // GalleryDbContext constructor'a geçirildi.
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var artworks = _context.Artworks.ToList();
        return View(artworks);
    }

    [HttpPost]
    public IActionResult Create(Artwork model)
    {
        if (ModelState.IsValid)
        {
            _context.Artworks.Add(model);
            _context.SaveChanges();

            // Başarı mesajını TempData'ya kaydediyoruz
            TempData["SuccessMessage"] = "Sanat eseri başarıyla eklendi!";

            // Kullanıcıyı "Index" action'ına yönlendiriyoruz
            return RedirectToAction("Index");
        }
        return View(model);
    }

    public IActionResult Edit(int id)
    {
        var artwork = _context.Artworks.Find(id);
        if (artwork == null) return NotFound();
        return View(artwork);
    }

    [HttpPost]
    public IActionResult Edit(Artwork model)
    {
        if (ModelState.IsValid)
        {
            _context.Artworks.Update(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(model);
    }

    public IActionResult Delete(int id)
    {
        var artwork = _context.Artworks.Find(id);
        if (artwork != null)
        {
            _context.Artworks.Remove(artwork);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }
}
