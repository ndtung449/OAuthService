using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthService.Core.Repositories
{
    public class ConfigurationRepository<T> : IConfigurationRepository<T>
        where T : class
    {
        public ConfigurationRepository(ConfigurationDbContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        protected ConfigurationDbContext Context { get; }

        protected DbSet<T> DbSet { get; }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return Context.Database.BeginTransaction();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public Task SaveChangesAsync()
        {
            return Context.SaveChangesAsync();
        }

        public IQueryable<T> Query()
        {
            return DbSet;
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }
    }
}
