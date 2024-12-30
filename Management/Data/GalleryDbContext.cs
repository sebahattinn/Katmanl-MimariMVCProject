using Microsoft.EntityFrameworkCore;
using OnlineGallery.Models;

namespace OnlineGallery.Data;

public class GalleryDbContext : DbContext
{
    public GalleryDbContext(DbContextOptions<GalleryDbContext> options) : base(options)
    {
    }

    // DbSet definitions
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Artwork> Artworks { get; set; }

    // Configuring relationships in OnModelCreating method
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Sales table - A sale is related to a user and an artwork
        modelBuilder.Entity<Sale>()
            .HasOne(s => s.User)
            .WithMany(u => u.Sales)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Delete sales when user is deleted

        modelBuilder.Entity<Sale>()
            .HasOne(s => s.Artwork)
            .WithMany(a => a.Sales)
            .HasForeignKey(s => s.ArtworkId)
            .OnDelete(DeleteBehavior.Cascade); // Delete sales when artwork is deleted

        modelBuilder.Entity<Sale>()
            .Property(s => s.Price)
            .HasColumnType("decimal(18,2)"); // Price için decimal(18,2) kullanarak SQL Server'a uygun hale getirdik


        // Comments table - A comment is related to a user and an artwork
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Delete comments when user is deleted

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Artwork)
            .WithMany(a => a.Comments)
            .HasForeignKey(c => c.ArtworkId)
            .OnDelete(DeleteBehavior.Cascade); // Delete comments when artwork is deleted

        // Category table - A category can have many artworks
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Artworks)
            .WithOne(a => a.Category)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Cascade); // Set category to NULL when category is deleted

        // User table - User's role is defined as a string
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(50); // Role field is required and cannot exceed 50 characters

        // Unique constraints (e.g., Name, Email)
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique(); // Ensure user email is unique

        modelBuilder.Entity<Artwork>()
            .HasIndex(a => a.Title)
            .IsUnique(); // Ensure artwork title is unique

        // Additional configurations
        modelBuilder.Entity<User>()
            .Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100); // Full name is required and cannot exceed 100 characters

        modelBuilder.Entity<Artwork>()
            .Property(a => a.Price)
            .HasColumnType("decimal(18,2)"); // Price is of decimal type

        modelBuilder.Entity<Category>()
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50); // Category name is required and cannot exceed 50 characters
    }
}
