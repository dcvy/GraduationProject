
using Kclinic.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kclinic.DataAccess;
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<Category> Categories {  get; set; }
    public DbSet<CoverType> CoverTypes { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<Trial> Trials { get; set; }
	public DbSet<OrderHeader> OrderHeaders { get; set; }
	public DbSet<OrderDetail> OrderDetail { get; set; }
    public DbSet<Launch> Launchs { get; set; }
    public DbSet<About> Abouts { get; set; }
    public DbSet<Function> Functions { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<Partner> Partners { get; set; }
    public DbSet<CateItem> CateItems { get; set; }
    public DbSet<Product> Products { get; set; }
}
