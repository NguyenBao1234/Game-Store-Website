using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
namespace POWStudio.Models;

public class GameStoreDbContext : IdentityDbContext
{
    public GameStoreDbContext(DbContextOptions<GameStoreDbContext> options) : base(options) {}
    public GameStoreDbContext(){}
    //Table Definition:
    public DbSet<Game> Game { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<GameCategory> GameCategory { get; set; }
    public DbSet<Order> Order { get; set; }
    public DbSet<OrderItem> OrderItem { get; set; }
    public DbSet<UserOrder> UserOrder { get; set; }
    public DbSet<Wishlist> Wishlist { get; set; }
    public DbSet<WishlistItem> WishlistItem { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer("Server=localhost;Database=GameStoreDB;Trusted_Connection=True;TrustServerCertificate=True");
    }
    
    //rename table from migration
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<IdentityUser>(b => b.ToTable("User"));
        builder.Entity<IdentityRole>(b => b.ToTable("Role"));
        builder.Entity<IdentityUserRole<string>>(b => b.ToTable("UserRole"));
        builder.Entity<IdentityUserClaim<string>>(b => b.ToTable("UserClaim"));
        builder.Entity<IdentityUserLogin<string>>(b => b.ToTable("UserLogin"));
        builder.Entity<IdentityRoleClaim<string>>(b => b.ToTable("RoleClaim"));
        builder.Entity<IdentityUserToken<string>>(b => b.ToTable("UserToken"));
        
        builder.Entity<GameCategory>().HasIndex(gc => new { gc.GameId, gc.CategoryId }).IsUnique();
    }
}