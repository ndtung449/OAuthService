using OAuthService.Core.Repositories;
using OAuthService.Domain.DTOs;
using System.Threading.Tasks;

namespace OAuthService.Core.Services
{
    public interface IApiResourceService
    {
        Task<PageResult<ApiResourceDto>> Get(int take = 100, int skip = 0);

        Task<ApiResourceDto> GetByName(string name);

        Task Create(ApiResourceCreateDto resource);

        Task Update(ApiResourceUpdateDto resource);

        Task Delete(string name);

        Task CreateApiSecret(string name, ApiSecretCreateDto form);
    }
}
