using OAuthService.Domain.DTOs;
using System.Threading.Tasks;

namespace OAuthService.Core.Services
{
    public interface IUserService
    {
        Task Create(UserCreateDto dto);
        Task Update(UserUpdateDto dto);
        Task<PageResult<UserDto>> Get(string name, bool isBlocked = false, int skip = 0, int take = 100);
        Task<UserDto> GetByUserName(string userName);
        Task Delete(string userName);
    }
}
