using OAuthService.Domain.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OAuthService.Core.Services
{
    public interface IClientService
    {
        Task<PageResult<ClientDto>> Get(string name, string grantType, int take = 100, int skip = 0);
        Task<ClientDto> GetByUri(string uri);
        Task<ClientCreatedDto> CreateHasRedirectUri(HasRedirectUriClientCreateDto dto, ICollection<string> grantTypes);
        Task<ClientCreatedDto> CreateNoRedirectUri(NoRedirectUriClientCreateDto dto, ICollection<string> grantTypes);
        Task<ClientDto> GetByClientId(string clientId);
        Task Update(ClientUpdateDto form);
        Task Delete(string clientId);
    }
}
