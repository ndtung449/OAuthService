using OAuthService.Core.Repositories;
using OAuthService.Domain.DTOs;
using IdentityServer4.EntityFramework.Entities;
using System.Threading.Tasks;

namespace OAuthService.Core.Services
{
    public interface IClientService
    {
        Task<PageResult<ClientViewModel>> Get(string name, string grantType, int take = 100, int skip = 0);
        Task<ClientViewModel> GetByUri(string uri);
        Task<ClientCreated> Create(Client client);
        Task<ClientViewModel> GetByClientId(string clientId);
        Task Update(UpdateClientForm form);
        Task Delete(string clientId);
    }
}
