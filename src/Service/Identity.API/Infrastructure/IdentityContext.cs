using Identity.API.Identity;
using Identity.API.Infrastructure.Identity;

namespace Identity.API.Infrastructure;

/// <remarks>
/// Add migrations using the following command inside the 'Catalog.API' project directory:
///
/// dotnet ef migrations add --context CatalogContext [migration-name]
/// </remarks>
public class IdentityContext : IdentityDbContextBase<
        ApplicationUser,
        ApplicationRole,
        ApplicationUserRole,
        Guid>
{
    public IdentityContext(DbContextOptions<IdentityContext> options, IConfiguration configuration) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder builder)
    {
    }
}
