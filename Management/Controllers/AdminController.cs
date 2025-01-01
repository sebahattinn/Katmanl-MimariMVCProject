using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;
using System.Linq;
using System.Threading.Tasks;

public class AdminController : Controller
{
    private readonly GalleryDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(GalleryDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Admin ana sayfası (Index sayfası)
    public IActionResult Index()
    {
        return View();  // Admin sayfası görüntülenir
    }

    // Kullanıcıları Listele (Ana Sayfa)
    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.ToListAsync();  // Kullanıcıları veritabanından al
        return View(users);  // Kullanıcıları ana sayfada görüntüle
    }

    // Kullanıcıyı Düzenle
    public async Task<IActionResult> EditUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(ApplicationUser user)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id);
            if (existingUser != null)
            {
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.UserName = user.UserName;

                // Diğer gerekli değişiklikler
                var result = await _userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Kullanıcı başarıyla güncellendi!";
                    return RedirectToAction("Users");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
        }
        return View(user);
    }

    // Kullanıcıyı Sil
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi!";
        }
        else
        {
            TempData["ErrorMessage"] = "Kullanıcı silinirken bir hata oluştu.";
        }
        return RedirectToAction("Users");
    }

    // Kullanıcı listesini döndüren aksiyon
    public async Task<IActionResult> GetUserList()
    {
        var users = await _userManager.Users.ToListAsync();  // Kullanıcıları al
        return PartialView("UserList", users);  // PartialView döndür
    }
}
