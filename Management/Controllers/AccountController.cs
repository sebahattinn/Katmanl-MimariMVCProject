using Microsoft.AspNetCore.Mvc;
using OnlineGallery.Models;
using OnlineGallery.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;  // PasswordHasher sınıfı için gerekli
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly GalleryDbContext _context;

    public AccountController(GalleryDbContext context)
    {
        _context = context;
    }

    // User Registration
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
            // PasswordHasher sınıfını kullanarak şifreyi hash'liyoruz
            var passwordHasher = new PasswordHasher<User>();

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Role = model.Role
            };

            // Şifreyi hash'leyip kaydediyoruz
            user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

            // Kullanıcıyı veritabanına ekliyoruz
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! You can now log in.";
            return RedirectToAction("Login");
        }
        return View(model);
    }

    // User Login
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user != null)
            {
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    // Giriş başarılı
                    if (user.Role == "admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid password.");
                }
            }
            else
            {
                ModelState.AddModelError("", "User not found.");
            }
        }

        return View(model);
    }

    // User Logout
    public IActionResult Logout()
    {
        // Perform logout manually
        TempData["SuccessMessage"] = "Successfully logged out.";
        return RedirectToAction("Index", "Home");
    }
}
