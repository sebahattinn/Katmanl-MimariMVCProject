using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using OnlineGallery.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Veritabaný baðlantýsý
builder.Services.AddDbContext<GalleryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity (Kimlik doðrulama ve yönetimi) services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<GalleryDbContext>()  // Kullanýcý veritabaný iþlemleri için GalleryDbContext kullanýyoruz
    .AddDefaultTokenProviders();  // Token saðlayýcýlarý ekliyoruz

// Authentication için Cookie yapýlandýrmasý
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Giriþ yapýlmadýðýnda yönlendirilmesi gereken sayfa
        options.AccessDeniedPath = "/Account/AccessDenied"; // Yetkisiz eriþim için sayfa
    });

// Authorization (Yetkilendirme) iþlemi
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin")); // Admin rolü gereksinimi
});

// MVC hizmetlerini ekleyin
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Geliþtirme ortamýnda hatalarý detaylý þekilde gösteriyor
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication ve Authorization iþlemleri
app.UseAuthentication();  // Kimlik doðrulama
app.UseAuthorization();   // Yetkilendirme

// Ana Controller yönlendirmesi
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();