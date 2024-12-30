using Microsoft.AspNetCore.Mvc;
using OnlineGallery.Data;
using OnlineGallery.Models;
using System.Linq;

public class CategoriesViewComponent : ViewComponent
{
    private readonly GalleryDbContext _context;

    public CategoriesViewComponent(GalleryDbContext context)
    {
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        var categories = _context.Categories.ToList();
        return View(categories);
    }
}
