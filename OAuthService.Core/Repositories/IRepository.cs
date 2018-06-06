using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthService.Core.Repositories
{
    public interface IRepository<T>
    {
        IQueryable<T> Query();

        void Add(T entity);

        IDbContextTransaction BeginTransaction();

        void SaveChanges();

        Task SaveChangesAsync();

        void Remove(T entity);
    }
}
