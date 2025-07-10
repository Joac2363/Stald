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
    public DbSet<Member> Members { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<Box> Boxes { get; set; }
}