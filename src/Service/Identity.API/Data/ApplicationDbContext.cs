using Identity.API.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;

namespace Identity.API.Data;

public class ApplicationDbContext
	: IdentityDbContextBase<
		ApplicationUser,
		ApplicationRole,
		ApplicationUserRole,
		string>,
	IApplicationDbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

	public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
	{
		return await Database.BeginTransactionAsync(cancellationToken);
	}

	public async Task CommitAsync(CancellationToken cancellationToken = default)
	{
		await SaveChangesAsync(cancellationToken);
		await Database.CommitTransactionAsync(cancellationToken);
	}

	public async Task RollbackAsync(CancellationToken cancellationToken = default)
	{
		await Database.RollbackTransactionAsync(cancellationToken);
	}

	public DbSet<Rb_CustomerUser> Rb_CustomerUsers => Set<Rb_CustomerUser>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}
}
