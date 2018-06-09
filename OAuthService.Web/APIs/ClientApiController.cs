using OAuthService.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using OAuthService.Domain.DTOs;
using IdentityServer4.Models;

namespace OAuthService.Web.APIs
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
            PageResult<ClientDto> items = await _clientService.Get(name, grantType, take, skip);
            
            return Ok(items);
        }

        [HttpGet("getByUri")]
        public async Task<IActionResult> GetByUri(string uri)
        {
            ClientDto client = await _clientService.GetByUri(uri);

            return Ok(client);
        }

        [HttpGet("{clientId}")]
        public async Task<IActionResult> GetByClientId(string clientId)
        {
            ClientDto client = await _clientService.GetByClientId(clientId);

            return Ok(client);
        }

        [HttpPost("ResourceOwner")]
        public async Task<IActionResult> CreateResourceOwnerPasswordClient([FromBody] NoRedirectUriClientCreateDto dto)
        {
            return await CreateNoRedirectUriClient(dto, GrantTypes.ResourceOwnerPassword);
        }

        [HttpPost("ClientCredentials")]
        public async Task<IActionResult> CreateClientCredentialsClient([FromBody] NoRedirectUriClientCreateDto dto)
        {
            return await CreateNoRedirectUriClient(dto, GrantTypes.ClientCredentials);
        }

        [HttpPost("AuthorizationCode")]
        public async Task<IActionResult> CreateAuthorizationCodeClient([FromBody] HasRedirectUriClientCreateDto dto)
        {
            return await CreateHasRedirectUriClient(dto, GrantTypes.Code);
        }

        [HttpPost("Implicit")]
        public async Task<IActionResult> CreateImplicitClient([FromBody] HasRedirectUriClientCreateDto dto)
        {
            return await CreateHasRedirectUriClient(dto, GrantTypes.Implicit);
        }

        [HttpPost("Hybrid")]
        public async Task<IActionResult> CreateHybridClient([FromBody] HasRedirectUriClientCreateDto dto)
        {
            return await CreateHasRedirectUriClient(dto, GrantTypes.HybridAndClientCredentials);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClient([FromBody] ClientUpdateDto form)
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

        private async Task<IActionResult> CreateNoRedirectUriClient(
            [FromBody] NoRedirectUriClientCreateDto dto,
            ICollection<string> grantTypes)
        {
            ClientCreatedDto result = await _clientService.CreateNoRedirectUri(dto, grantTypes);
            string uri = Url.Action(nameof(GetByClientId), new { result.ClientId });

            return Created(uri, result);
        }

        private async Task<IActionResult> CreateHasRedirectUriClient(
            [FromBody] HasRedirectUriClientCreateDto dto,
            ICollection<string> grantTypes)
        {
            ClientCreatedDto result = await _clientService.CreateHasRedirectUri(dto, grantTypes);
            string uri = Url.Action(nameof(GetByClientId), new { result.ClientId });

            return Created(uri, result);
        }
    }
}
