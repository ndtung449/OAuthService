using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using OAuthService.Domain.DTOs;
using OAuthService.Core.Services;

namespace OAuthService.Admin.APIs
{
    [Route("api/clients/{clientId}/profile")]
    public class ClientProfileApiController : Controller
    {
        private readonly IClientProfileService _clientProfileService;

        public ClientProfileApiController(IClientProfileService clientProfileService)
        {
            _clientProfileService = clientProfileService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string clientId)
        {
            ClientProfileDto profile = await _clientProfileService.Get(clientId);

            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string clientId, [FromBody] ClientProfileCreateDto form)
        {
            await _clientProfileService.Create(clientId, form);
            string uri = Url.Action(nameof(Get), new { clientId });

            return Created(uri, null);
        }

        [HttpPut]
        public async Task<IActionResult> Update(string clientId, [FromBody] ClientProfileUpdateDto form)
        {
            await _clientProfileService.Update(clientId, form);

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string clientId)
        {
            await _clientProfileService.Delete(clientId);

            return NoContent();
        }
    }
}
