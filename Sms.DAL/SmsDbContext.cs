using Microsoft.EntityFrameworkCore;
using SmsConsole.DAL.DTOs;

namespace Sms.ConsoleApp.Data;

public class SmsDbContext : DbContext
{
    public DbSet<MenuItemDto> MenuItems { get; set; }

    public SmsDbContext(DbContextOptions<SmsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MenuItemDto>(entity =>
        {
            entity.ToTable("menu_items");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Article).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.FullPath).HasMaxLength(500);
        });
    }
}