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
        // Kullanıcı var mı kontrol et, yoksa yeni bir kullanıcı nesnesi oluştur
        var user = userId.HasValue ? await _context.Users.FindAsync(userId.Value) : new User();

        // EditUserPartialView görümünü döndür
        return PartialView("EditUserPartialView", user);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(User user)
    {
        if (!ModelState.IsValid)
        {
            // Geçersizlik durumunda hata mesajlarını döndür
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            TempData["ErrorMessage"] = "Geçersiz giriş, lütfen tüm alanları doğru doldurduğunuzdan emin olun.";
            return PartialView("EditUserPartialView", user);
        }

        try
        {
            if (user.UserId == 0)
            {
                // Yeni kullanıcı ekliyoruz
                user.ProfileImageUrl ??= "default-image-url.jpg"; // Varsayılan resim yolunu kontrol et
                _context.Users.Add(user);
                TempData["SuccessMessage"] = "Yeni kullanıcı başarıyla eklendi!";
            }
            else
            {
                // Kullanıcıyı güncelliyoruz
                var existingUser = await _context.Users.FindAsync(user.UserId);

                if (existingUser != null)
                {
                    // Kullanıcı bilgilerini güncelle
                    existingUser.FullName = user.FullName;
                    existingUser.Email = user.Email;
                    existingUser.Address = user.Address; // Adres alanını güncelle
                    existingUser.Role = user.Role;
                    existingUser.ProfileImageUrl = user.ProfileImageUrl ?? existingUser.ProfileImageUrl; // Null kontrolü

                    _context.Users.Update(existingUser);
                    TempData["SuccessMessage"] = "Kullanıcı başarıyla güncellendi!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Kullanıcı bulunamadı!";
                    return PartialView("EditUserPartialView", user);
                }
            }

            // Değişiklikleri kaydet
            await _context.SaveChangesAsync();

            // Başarılı işlem sonrası Admin/Index sayfasına yönlendir
            return RedirectToAction("Index", "Admin");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
            return PartialView("EditUserPartialView", user);
        }
    }

    // Yeni kullanıcı ekleme işlemi
    public IActionResult AddUser()
    {
        // Yeni bir kullanıcı eklemek için boş bir User nesnesi oluşturuyoruz
        return PartialView("EditUserPartialView", new User());
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(User user)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Geçersiz giriş, lütfen tüm alanları doğru doldurduğunuzdan emin olun.";
            return PartialView("EditUserPartialView", user);
        }

        try
        {
            user.ProfileImageUrl ??= "default-image-url.jpg"; // Varsayılan resim yolunu kontrol et
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Yeni kullanıcı başarıyla eklendi!";
            return RedirectToAction("Index", "Admin");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
            return PartialView("EditUserPartialView", user);
        }
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
        return Json(new { success = true });  //bu jsonu View olarak yazmayı denedim pek bir şey değişmedi.
    }
}
