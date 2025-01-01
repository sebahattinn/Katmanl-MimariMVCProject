using Microsoft.AspNetCore.Mvc;
using OnlineGallery.Models;
using OnlineGallery.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly GalleryDbContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(GalleryDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // Kullanıcı Kaydı
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
                Role = model.Role, // Rolü kaydet
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

    // Kullanıcı Girişi
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

            // Kullanıcı var mı, şifre doğru mu, rolü kontrol et
            if (user != null)
            {
                // Şifre doğruysa
                if (BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    // Kullanıcıyı doğruladıktan sonra rolüne göre yönlendir
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    if (user.Role == "admin")  // Admin rolüne sahipse Admin paneline yönlendir    /Buradaki A büyük diye çok acılar çektim
                    {
                        return RedirectToAction("Index", "Admin");  // Admin paneline yönlendirme
                    }

                    // Diğer kullanıcılar için Home Index sayfasına yönlendir
                    return RedirectToAction("Index", "Home");  // Ana sayfaya yönlendirme
                }
                else
                {
                    // Şifre yanlış
                    ModelState.AddModelError("", "Geçersiz şifre.");
                }
            }
            else
            {
                // Kullanıcı bulunamadı
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
            }
        }

        return View(model);
    }

    // Çıkış işlemi
    public async Task<IActionResult> Logout()
    {
        // Kullanıcıyı çıkış yaptır
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
