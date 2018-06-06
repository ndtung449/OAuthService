using OAuthService.Core.Services;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OAuthService.Domain.DTOs;

namespace OAuthService.Api.Controllers
{
    [Route("api/clients")]
    public class ClientApiController : Controller
    {
        private readonly IClientService _clientService;

        public ClientApiController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string name, string grantType, int take = 100, int skip = 0)
        {
            PageResult<ClientViewModel> items = await _clientService.Get(name, grantType, take, skip);
            
            return Ok(items);
        }

        [HttpGet("getByUri")]
        public async Task<IActionResult> GetByUri(string uri)
        {
            ClientViewModel client = await _clientService.GetByUri(uri);

            return Ok(client);
        }

        [HttpGet("{clientId}")]
        public async Task<IActionResult> GetByClientId(string clientId)
        {
            ClientViewModel client = await _clientService.GetByClientId(clientId);

            return Ok(client);
        }

        [HttpPost("ResourceOwner")]
        public async Task<IActionResult> CreateResourceOwnerPasswordClient([FromBody] NoRedirectUriClientForm form)
        {
            return await CreateNoRedirectUriClient(form, IdentityServer4.Models.GrantTypes.ResourceOwnerPassword);
        }

        [HttpPost("ClientCredentials")]
        public async Task<IActionResult> CreateClientCredentialsClient([FromBody] NoRedirectUriClientForm form)
        {
            return await CreateNoRedirectUriClient(form, IdentityServer4.Models.GrantTypes.ClientCredentials);
        }

        [HttpPost("AuthorizationCode")]
        public async Task<IActionResult> CreateAuthorizationCodeClient([FromBody] HasRedirectUriClientForm form)
        {
            return await CreateHasRedirectUriClient(form, IdentityServer4.Models.GrantTypes.Code);
        }

        [HttpPost("Implicit")]
        public async Task<IActionResult> CreateImplicitClient([FromBody] HasRedirectUriClientForm form)
        {
            return await CreateHasRedirectUriClient(form, IdentityServer4.Models.GrantTypes.Implicit);
        }

        [HttpPost("Hybrid")]
        public async Task<IActionResult> CreateHybridClient([FromBody] HasRedirectUriClientForm form)
        {
            return await CreateHasRedirectUriClient(form, IdentityServer4.Models.GrantTypes.HybridAndClientCredentials);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClient([FromBody] UpdateClientForm form)
        {
            await _clientService.Update(form);

            return NoContent();
        }

        [HttpDelete("{clientId}")]
        public async Task<IActionResult> DeleteClient(string clientId)
        {
            await _clientService.Delete(clientId);

            return NoContent();
        }

        private async Task<IActionResult> CreateNoRedirectUriClient([FromBody] NoRedirectUriClientForm form, ICollection<string> grantTypes)
        {
            Client client = BuildClient(
                form.Name,
                form.Uri,
                form.Scopes,
                grantTypes);

            ClientCreated result = await _clientService.Create(client);
            string uri = Url.Action(nameof(GetByClientId), new { result.ClientId });

            return Created(uri, result);
        }

        private async Task<IActionResult> CreateHasRedirectUriClient([FromBody] HasRedirectUriClientForm form, ICollection<string> grantTypes)
        {
            Client client = BuildClient(
                form.Name,
                form.Uri,
                form.Scopes,
                grantTypes,
                form.RedirectUri,
                form.PostLogoutRedirectUri);

            ClientCreated result = await _clientService.Create(client);
            string uri = Url.Action(nameof(GetByClientId), new { result.ClientId });

            return Created(uri, result);
        }

        private Client BuildClient(
            string name,
            string uri,
            List<string> scopes,
            ICollection<string> grantTypes,
            string redirectUri = null,
            string postLogoutRedirectUri = null)
        {
            return new Client
            {
                ClientName = name,
                ClientUri = uri,
                AllowedScopes = scopes.Select(scope => new ClientScope { Scope = scope }).ToList(),
                AllowedGrantTypes = grantTypes.Select(granType => new ClientGrantType { GrantType = granType }).ToList(),
                RedirectUris = redirectUri != null
                    ? new List<ClientRedirectUri> { new ClientRedirectUri { RedirectUri = redirectUri } }
                    : new List<ClientRedirectUri>(),
                PostLogoutRedirectUris = postLogoutRedirectUri != null
                    ? new List<ClientPostLogoutRedirectUri> { new ClientPostLogoutRedirectUri { PostLogoutRedirectUri = postLogoutRedirectUri } }
                    : new List<ClientPostLogoutRedirectUri>(),
            };
        }
    }
}
