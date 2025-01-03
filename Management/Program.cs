using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using OnlineGallery.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Veritaban� ba�lant�s�
builder.Services.AddDbContext<GalleryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity (Kimlik do�rulama ve y�netimi) services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<GalleryDbContext>()  // Kullan�c� veritaban� i�lemleri i�in GalleryDbContext kullan�yoruz
    .AddDefaultTokenProviders();  // Token sa�lay�c�lar� ekliyoruz

// Authentication i�in Cookie yap�land�rmas�
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Giri� yap�lmad���nda y�nlendirilmesi gereken sayfa
        options.AccessDeniedPath = "/Account/AccessDenied"; // Yetkisiz eri�im i�in sayfa
    });

// Authorization (Yetkilendirme) i�lemi
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin")); // Admin rol� gereksinimi
});

// MVC hizmetlerini ekleyin
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Geli�tirme ortam�nda hatalar� detayl� �ekilde g�steriyor
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication ve Authorization i�lemleri
app.UseAuthentication();  // Kimlik do�rulama
app.UseAuthorization();   // Yetkilendirme

// Ana Controller y�nlendirmesi
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();