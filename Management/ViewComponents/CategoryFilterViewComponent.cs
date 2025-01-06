using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineGallery.ViewComponents
{
    public class CategoryFilterViewComponent : ViewComponent
    {
        private readonly GalleryDbContext _context;

        public CategoryFilterViewComponent(GalleryDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var viewModel = new CategoryFilterViewModel
            {
                Categories = await _context.Categories.ToListAsync(),
             //   Artworks = await _context.Artworks.ToListAsync() // Sanat eserlerini de ekliyoruz sakın burayı çalıştırma
            };

            return View(viewModel);
        }
    }
}