using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VanManager.Domain.Constants;

namespace VanManager.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.HasData(
            new IdentityRole<Guid>
            {                
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = Roles.Admin,
                NormalizedName = Roles.Admin.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new IdentityRole<Guid>
            {                
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = Roles.FleetOwner,
                NormalizedName = Roles.FleetOwner.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new IdentityRole<Guid>
            {                
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = Roles.Driver,
                NormalizedName = Roles.Driver.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = Roles.Parent,
                NormalizedName = Roles.Parent.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );
    }
}