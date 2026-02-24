using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1.Data;

public class LibraryContext : DbContext
{
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Genre> Genres { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=library.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().Property(a => a.FirstName).IsRequired().HasMaxLength(50);
        modelBuilder.Entity<Author>().Property(a => a.LastName).IsRequired().HasMaxLength(50);
        
        modelBuilder.Entity<Book>().Property(b => b.Title).IsRequired().HasMaxLength(200);
        modelBuilder.Entity<Book>().Property(b => b.ISBN).HasMaxLength(20);

        modelBuilder.Entity<Book>()
            .HasMany(b => b.Authors)
            .WithMany(a => a.Books);

        modelBuilder.Entity<Book>()
            .HasMany(b => b.Genres)
            .WithMany(g => g.Books);
    }
}