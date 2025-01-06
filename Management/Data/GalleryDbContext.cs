using Microsoft.EntityFrameworkCore;  // EF Core için gerekli
using OnlineGallery.Models;  // User, Artwork ve diğer model sınıfları

namespace OnlineGallery.Data
{
    public class GalleryDbContext : DbContext  
    {
        public GalleryDbContext(DbContextOptions<GalleryDbContext> options) : base(options)
        {
        }

        // DbSet tanımlamaları
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Artwork> Artworks { get; set; }  
        public DbSet<User> Users { get; set; } 

        // Veritabanı ilişkilerini OnModelCreating metodunda yapılandırıyoruz
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Identity kısmı kaldırıldı

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

            // User tablosu - Kullanıcı silindiğinde ilişkili veriler de silinsin
            modelBuilder.Entity<User>()
                .HasMany(u => u.Sales)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silindiğinde satışlar silinsin

            modelBuilder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silindiğinde yorumlar silinsin

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

            // Entity'lerin tablo adlarını belirtiyoruz
            modelBuilder.Entity<Artwork>().ToTable("Artworks");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Comment>().ToTable("Comments");
            modelBuilder.Entity<Sale>().ToTable("Sales");
        }
    }
}
