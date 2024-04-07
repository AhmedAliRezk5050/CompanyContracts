using System.Reflection;
using Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Contract> Contracts { get; set; } = null!;
    public DbSet<Funder> Funders { get; set; } = null!;
    public DbSet<InstallmentPayment> InstallmentPayments { get; set; } = null!;
    public DbSet<Destruction> Destructions { get; set; } = null!;
    public DbSet<AppUser> AppUsers { get; set; } = null!;
    
    public AppDbContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}