using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.API.Data;

public interface IApplicationDbContext
{
	DbSet<TEntity> Set<TEntity>() where TEntity : class;
	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
	Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
	Task CommitAsync(CancellationToken cancellationToken = default);
	Task RollbackAsync(CancellationToken cancellationToken = default);
}
