using API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class DbContext : IdentityDbContext<User>
{
    public DbContext(DbContextOptions<DbContext> options)
        : base(options)
    {

    }
    public DbSet<Stable> Stables { get; set; }
    public DbSet<Media> Media { get; set; }
    public DbSet<Post> Posts { get; set; }

    // Define your DbSets:
    // public DbSet<Product> Products { get; set; }
}