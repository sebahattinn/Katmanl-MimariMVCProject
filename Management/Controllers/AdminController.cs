using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;
using System.Linq;
using System.Threading.Tasks;

public class AdminController : Controller
{
    private readonly GalleryDbContext _context;

    public AdminController(GalleryDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Users()
    {
        var users = await _context.Users.ToListAsync();
        return View(users);
    }

    public async Task<IActionResult> GetUserList()
    {
        var users = await _context.Users.ToListAsync();
        return PartialView("UserList", users);
    }

    public async Task<IActionResult> EditUser(int? userId)
    {
        var user = userId.HasValue ? await _context.Users.FindAsync(userId.Value) : new User();
        return PartialView("EditUserPartialView", user);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(User user)
    {
        if (ModelState.IsValid)
        {
            if (user.UserId == 0)
            {
                _context.Users.Add(user);
                TempData["SuccessMessage"] = "Yeni kullanıcı başarıyla eklendi!";
            }
            else
            {
                var existingUser = await _context.Users.FindAsync(user.UserId);
                if (existingUser != null)
                {
                    existingUser.FullName = user.FullName;
                    existingUser.Email = user.Email;
                    _context.Users.Update(existingUser);
                    TempData["SuccessMessage"] = "Kullanıcı başarıyla güncellendi!";
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Users");
        }
        return PartialView("EditUserPartialView", user);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser([FromBody] int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return Json(new { success = false, message = "Kullanıcı bulunamadı." });
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }
}
