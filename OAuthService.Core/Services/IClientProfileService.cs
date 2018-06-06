using OAuthService.Domain.DTOs;
using System.Threading.Tasks;

namespace OAuthService.Core.Services
{
    public interface IClientProfileService
    {
        Task<ClientProfileViewModel> Get(string clientId);
        Task Create(string clientId, ClientProfileForm form);
        Task Update(string clientId, ClientProfileUpdateForm form);
        Task Delete(string clientId);
    }
}
