using Microsoft.AspNetCore.Mvc;
using OnlineGallery.Models;
using OnlineGallery.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

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
            // Admin rolünü doğrudan seçmeye çalışanları engelle
            if (model.Role == "Admin")
            {
                ModelState.AddModelError(string.Empty, "Admin rolü doğrudan seçilemez!");
                return View(model);
            }

            var passwordHasher = new PasswordHasher<User>();

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Role = model.Role ?? "User" // Varsayılan olarak "User" rolü atanır
            };

            user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kayıt işlemi başarılı! Giriş yapabilirsiniz.";
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user != null)
            {
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    var token = GenerateToken(user.UserId, user.Role);

                    // Store token in TempData
                    TempData["AuthToken"] = token;

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Geçersiz şifre.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
            }
        }

        return View(model);
    }

    // Generate JWT token
    private string GenerateToken(int user_id, string role)
    {
        var claims = new[] {
            new Claim("user_id", user_id.ToString()),
            new Claim("role", role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TabiSizAnneleriTarafındanSizeEmanetEdilenCocuklariHerBakimdanYetersizGordugunuz"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "application",
            audience: "application",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
}
