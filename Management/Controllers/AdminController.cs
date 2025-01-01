using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OnlineGallery.Data;
using OnlineGallery.Models;
using Microsoft.AspNetCore.Identity;

namespace OnlineGallery.Controllers
{
    [Authorize(Roles = "Admin")] // Yalnızca "Admin" rolüne sahip kullanıcılar erişebilir
    public class AdminController : Controller
    {
        private readonly GalleryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(GalleryDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Kullanıcıları Listele (Ana Sayfa)
        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();  // Kullanıcıları veritabanından al
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
                    await _userManager.UpdateAsync(existingUser);
                    TempData["SuccessMessage"] = "Kullanıcı başarıyla güncellendi!";
                    return RedirectToAction("Users");
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
            return RedirectToAction("Users");
        }
    }
}
