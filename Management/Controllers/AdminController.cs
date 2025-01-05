using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

public class AdminController : Controller
{
    private readonly GalleryDbContext _context;

    public AdminController(GalleryDbContext context)
    {
        _context = context;
    }

    // Index Action
    public IActionResult Index()
    {
        return View();
    }

    // Users Action
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

    // Edit User Action
    public async Task<IActionResult> EditUser(int? userId)
    {
        var user = userId.HasValue ? await _context.Users.FindAsync(userId.Value) : new User();
        return PartialView("EditUserPartialView", user);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(User user)
    {
        if (!ModelState.IsValid)
        {
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
                user.ProfileImageUrl ??= "default-image-url.jpg";
                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, user.PasswordHash);
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
                    existingUser.Address = user.Address;
                    existingUser.Role = user.Role;
                    existingUser.ProfileImageUrl = user.ProfileImageUrl ?? existingUser.ProfileImageUrl;

                    var passwordHasher = new PasswordHasher<User>();
                    existingUser.PasswordHash = passwordHasher.HashPassword(existingUser, user.PasswordHash);

                    _context.Users.Update(existingUser);
                    TempData["SuccessMessage"] = "Kullanıcı başarıyla güncellendi!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Kullanıcı bulunamadı!";
                    return PartialView("EditUserPartialView", user);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Admin");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
            return PartialView("EditUserPartialView", user);
        }
    }

    public IActionResult AddUser()
    {
        return PartialView("AddUserPartialView", new User());
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
            user.ProfileImageUrl ??= "default-image-url.jpg";
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, user.PasswordHash);
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
        return Json(new { success = true });
    }

    // Add Artwork Action
    public async Task<IActionResult> AddArtwork()
    {
        var categories = await _context.Categories.ToListAsync();
        ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");

        var defaultCategory = categories.FirstOrDefault();
        var newArtwork = new Artwork
        {
            Title = "Default Title",
            Description = "Default Description",
            ImageUrl = "default-image-url.jpg",
            Category = defaultCategory ?? new Category { CategoryId = 1, Name = "Default Category" },
            CategoryId = defaultCategory?.CategoryId ?? 1
        };

        return PartialView("AddArtworkPartialView", newArtwork);
    }

    [HttpPost]
    public async Task<IActionResult> AddArtwork(Artwork artwork)
    {
        var categories = await _context.Categories.ToListAsync();
        ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            TempData["ErrorMessage"] = "Invalid input, please ensure all fields are filled out correctly.";
            return PartialView("AddArtworkPartialView", artwork);
        }

        try
        {
            var selectedCategory = await _context.Categories.FindAsync(artwork.CategoryId);
            if (selectedCategory == null)
            {
                TempData["ErrorMessage"] = "Selected category not found.";
                return PartialView("AddArtworkPartialView", artwork);
            }

            artwork.Category = selectedCategory;

            _context.Artworks.Add(artwork);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "New artwork added successfully!";
            return RedirectToAction("Index", "Admin");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            return PartialView("AddArtworkPartialView", artwork);
        }
    }

    // Edit Artwork by Title Action
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> EditArtworkByTitle(string title)
    {
        var artwork = await _context.Artworks.FirstOrDefaultAsync(a => a.Title == title);
        if (artwork == null)
        {
            TempData["ErrorMessage"] = "Sanat eseri bulunamadı!";
            return RedirectToAction("Index", "Admin");
        }

        var categories = await _context.Categories.ToListAsync();
        if (categories == null || !categories.Any())
        {
            TempData["ErrorMessage"] = "Kategori mevcut değil. Lütfen önce kategori ekleyin.";
            return RedirectToAction("Index", "Admin");
        }

        ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
        return PartialView("EditArtworkPartialView", artwork);
    }

    [HttpPost]
    public async Task<IActionResult> EditArtworkByTitle(Artwork artwork)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            TempData["ErrorMessage"] = "Geçersiz giriş, lütfen tüm alanları doğru doldurduğunuzdan emin olun.";
            return PartialView("EditArtworkPartialView", artwork);
        }

        try
        {
            var existingArtwork = await _context.Artworks.FindAsync(artwork.ArtworkId);

            if (existingArtwork != null)
            {
                existingArtwork.Title = artwork.Title;
                existingArtwork.Description = artwork.Description;
                existingArtwork.Price = artwork.Price;
                existingArtwork.CategoryId = artwork.CategoryId;
                existingArtwork.ImageUrl = artwork.ImageUrl;

                _context.Artworks.Update(existingArtwork);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Sanat eseri başarıyla güncellendi!";
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                TempData["ErrorMessage"] = "Sanat eseri bulunamadı!";
                return PartialView("EditArtworkPartialView", artwork);
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
            return PartialView("EditArtworkPartialView", artwork);
        }
    }

    // Delete Artwork by Title Action
    [HttpPost]
    public async Task<IActionResult> DeleteArtworkByTitle([FromBody] dynamic data)
    {
        string title = data?.title;

        // Log the title received
        Console.WriteLine($"Received title: {title}");

        if (string.IsNullOrEmpty(title))
        {
            return Json(new { success = false, message = "Bu İsimde Bir Görsel Bulunamadı" });
        }

        // Perform a case-insensitive search for the artwork
        var artwork = await _context.Artworks
            .FirstOrDefaultAsync(a => a.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

        if (artwork == null)
        {
            return Json(new { success = false, message = "Sanat eseri bulunamadı." });
        }

        // Log the artwork found
        Console.WriteLine($"Found artwork: {artwork.Title}");

        _context.Artworks.Remove(artwork);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }
}
