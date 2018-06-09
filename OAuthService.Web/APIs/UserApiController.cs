using Microsoft.AspNetCore.Mvc;
using OAuthService.Core.Services;
using OAuthService.Domain.DTOs;
using System.Threading.Tasks;

namespace OAuthService.Web.APIs
{
    [Route("api/users")]
    public class UserApiController : Controller
    {
        private readonly IUserService _userService;

        public UserApiController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string name, bool isBlocked = false, int skip = 0, int take = 100)
        {
            PageResult<UserDto> result = await _userService.Get(name, isBlocked, skip, take);
            return Ok(result);
        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> GetByUserName(string userName)
        {
            UserDto user = await _userService.GetByUserName(userName);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            await _userService.Create(dto);
            return Created(Url.Action(nameof(GetByUserName), new { dto.UserName }), null);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto dto)
        {
            await _userService.Update(dto);
            return NoContent();
        }

        [HttpDelete("{userName}")]
        public async Task<IActionResult> Delete(string userName)
        {
            await _userService.Delete(userName);
            return NoContent();
        }
    }
}
