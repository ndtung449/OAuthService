using OAuthService.Core.Repositories;
using OAuthService.Domain.DTOs;
using System.Threading.Tasks;

namespace OAuthService.Core.Services
{
    public interface IApiResourceService
    {
        Task<PageResult<ApiResourceViewModel>> Get(int take = 100, int skip = 0);

        Task<ApiResourceViewModel> GetByName(string name);

        Task Create(ApiResourceForm resource);

        Task Update(UpdateApiResourceForm resource);

        Task Delete(string name);

        Task CreateApiSecret(string name, ApiSecretForm form);
    }
}
