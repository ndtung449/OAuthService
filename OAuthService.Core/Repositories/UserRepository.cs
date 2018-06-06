using OAuthService.Domain.Entities;

namespace OAuthService.Core.Repositories
{
    public class UserRepository : Repository<User>, IRepository<User>
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
