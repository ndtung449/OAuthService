using OAuthService.Domain.DTOs;
using System.Threading.Tasks;

namespace OAuthService.Core.Services
{
    public interface IClientProfileService
    {
        Task<ClientProfileDto> Get(string clientId);
        Task Create(string clientId, ClientProfileCreateDto form);
        Task Update(string clientId, ClientProfileUpdateDto form);
        Task Delete(string clientId);
    }
}
