using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Identity.API.Infrastructure.Identity
{
    public abstract class IdentityDbContextBase<TIdentityUser, TIdentityRole, TIdentityUserRole, TKey>
         : IdentityDbContext<
             TIdentityUser,
             TIdentityRole,
             TKey,
             IdentityUserClaim<TKey>,
             TIdentityUserRole, IdentityUserLogin<TKey>,
             IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>
         where TIdentityUser : IdentityUser<TKey>
         where TIdentityRole : IdentityRole<TKey>
         where TIdentityUserRole : IdentityUserRole<TKey>
         where TKey : IEquatable<TKey>
    {
        public IdentityDbContextBase(DbContextOptions options) : base(options)
        {

        }
        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            int returnValue;
            try
            {
                returnValue = await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Resolve the concurrency conflict by refreshing the  
                // object context before re-saving changes. 
                return ResolveUpdateConcurrency(ex.Entries);
            }
            return returnValue;
        }
        public override int SaveChanges()
        {
            int returnValue;
            try
            {
                returnValue = base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Resolve the concurrency conflict by refreshing the  
                // object context before re-saving changes. 
                return ResolveUpdateConcurrency(ex.Entries);
            }
            return returnValue;
        }

        private Exception GetInnerestException(Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex;
        }

        private int ResolveUpdateConcurrency(IEnumerable<EntityEntry> entityEntries, int retryCount = 1)
        {
            try
            {
                //Need to wait 5ms before do retry to avoId) many requests at the same time
                Thread.Sleep(5);
                return base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                if (retryCount <= 5)
                {
                    return ResolveUpdateConcurrency(exception.Entries, retryCount + 1);
                }
                throw;
            }

        }

        public void SetState(object entity, EntityState entityState)
        {
            Entry(entity).State = entityState;
        }

        public void SetPropertyModifiedState<TEntity, TProperty>(TEntity entity, params Expression<Func<TEntity, TProperty>>[] excludeProperties) where TEntity : class
        {
            var entry = Entry(entity);
            foreach (var item in excludeProperties)
            {
                entry.Property(item).IsModified = false;
            }
        }
    }
}