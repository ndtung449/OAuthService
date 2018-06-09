using Microsoft.AspNetCore.Mvc;
using OAuthService.Domain.DTOs;
using System.Threading.Tasks;
using OAuthService.Core.Services;

namespace OAuthService.Web.APIs
{
    [Route("api/apiResources")]
    public class ApiResourceApiController : Controller
    {
        private readonly IApiResourceService _apiResourceService;

        public ApiResourceApiController(IApiResourceService apiResourceService)
        {
            _apiResourceService = apiResourceService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int take = 100, int skip = 0)
        {
            PageResult<ApiResourceDto> items = await _apiResourceService.Get(take, skip);

            return Ok(items);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetByName (string name)
        {
            ApiResourceDto item = await _apiResourceService.GetByName(name);

            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApiResourceCreateDto form)
        {
            await _apiResourceService.Create(form);
            string uri = Url.Action(nameof(GetByName), new { form.Name });

            return Created(uri, null);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ApiResourceUpdateDto form)
        {
            await _apiResourceService.Update(form);

            return NoContent();
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            await _apiResourceService.Delete(name);

            return NoContent();
        }

        [HttpPost("{name}/createSecret")]
        public async Task<IActionResult> CreateSecret(string name, [FromBody] ApiSecretCreateDto form)
        {
            await _apiResourceService.CreateApiSecret(name, form);

            return Ok();
        }
    }
}
