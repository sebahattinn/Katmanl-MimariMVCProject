using Microsoft.AspNetCore.Mvc;
using OnlineGallery.Models;
using OnlineGallery.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly GalleryDbContext _context;

    public AccountController(GalleryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Kullanıcıyı oluştur
            var user = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password), // Şifreyi hashle
                Role = model.Role,
                UserName = model.Email // UserName property is required by Identity
            };
            // Kullanıcıyı veritabanına ekle
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Kullanıcı kaydedildikten sonra giriş işlemi
            return RedirectToAction("Login");
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // E-posta ile kullanıcıyı bul
            var user = await _context.Users
                                      .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                // Şifre doğru ise giriş yapmış say
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
        }
        return View(model);
    }

    public IActionResult Logout()
    {
        // Çıkış işlemi
        return RedirectToAction("Index", "Home");
    }
}