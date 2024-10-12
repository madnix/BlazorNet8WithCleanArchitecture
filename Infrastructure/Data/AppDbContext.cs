using Domain.Entity.Authentication;
using Domain.Entity.VehicleEntity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    /// <inheritdoc />
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<RefreshToken> RefreshTokens { get; init; }
    public DbSet<Vehicle> Vehicles { get; init; }
    public DbSet<VehicleBrand> VehicleBrands { get; init; }
    public DbSet<VehicleOwner> VehicleOwners { get; init; }
}