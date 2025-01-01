using Microsoft.AspNetCore.Identity;  // Identity için gerekli
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;  // EF Core için gerekli
using OnlineGallery.Models;  // ApplicationUser sınıfını kullanabilmek için gerekli

namespace OnlineGallery.Data;

public class GalleryDbContext : IdentityDbContext<ApplicationUser>  // ApplicationUser'ı kullanıyoruz
{
    public GalleryDbContext(DbContextOptions<GalleryDbContext> options) : base(options)
    {
    }

    // DbSet tanımlamaları
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Artwork> Artworks { get; set; }

    // Veritabanı ilişkilerini OnModelCreating metodunda yapılandırıyoruz
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Identity için gerekli

        // Sales tablosu - Bir satış, bir kullanıcıya ve bir esere bağlıdır
        modelBuilder.Entity<Sale>()
            .HasOne(s => s.User)
            .WithMany(u => u.Sales)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silindiğinde satışları da sil

        modelBuilder.Entity<Sale>()
            .HasOne(s => s.Artwork)
            .WithMany(a => a.Sales)
            .HasForeignKey(s => s.ArtworkId)
            .OnDelete(DeleteBehavior.Cascade); // Eser silindiğinde satışları da sil

        modelBuilder.Entity<Sale>()
            .Property(s => s.Price)
            .HasColumnType("decimal(18,2)"); // Fiyat için decimal(18,2) kullanarak SQL Server'a uygun hale getirdik


        // Comments tablosu - Bir yorum, bir kullanıcıya ve bir esere bağlıdır
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silindiğinde yorumları da sil

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Artwork)
            .WithMany(a => a.Comments)
            .HasForeignKey(c => c.ArtworkId)
            .OnDelete(DeleteBehavior.Cascade); // Eser silindiğinde yorumları da sil

        // Category tablosu - Bir kategori, birçok eseri barındırabilir
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Artworks)
            .WithOne(a => a.Category)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Cascade); // Kategori silindiğinde eserlerin kategori bilgisi NULL olur

        // Kullanıcı tablosu - Kullanıcının rolü bir string olarak tanımlanmış
        modelBuilder.Entity<ApplicationUser>()  // User yerine ApplicationUser kullanıyoruz
            .Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100); // Kullanıcı adı zorunlu ve 100 karakteri geçemez

        // Kullanıcıların e-posta adreslerinin benzersiz olmasını sağlıyoruz
        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email)
            .IsUnique(); // E-posta adresi benzersiz olmalı

        // Artwork tablosu - Eserin başlığının benzersiz olmasını sağlıyoruz
        modelBuilder.Entity<Artwork>()
            .HasIndex(a => a.Title)
            .IsUnique(); // Eser başlığı benzersiz olmalı

        // Fiyat alanı ve diğer ek özellikler
        modelBuilder.Entity<Artwork>()
            .Property(a => a.Price)
            .HasColumnType("decimal(18,2)"); // Fiyat alanı decimal(18,2) olarak tanımlanmış

        modelBuilder.Entity<Category>()
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50); // Kategori adı zorunlu ve 50 karakteri geçemez
    }
}
