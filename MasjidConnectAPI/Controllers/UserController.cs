using MasjidConnect.Model.Request.User;
using Microsoft.AspNetCore.Mvc;
using MasjidConnect.Application.Interfaces;

namespace MasjidConnectAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(RegisterRequest model)
        {
            var response =  await _userRepository.RegisterUserAsync(model);
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginRequest model)
        {
            var response = await _userRepository.ValidateUserAsync(model);
            return Ok(response);
        }

        [HttpPost("GetUser")]
        public async Task<IActionResult> GetUserAsync(int Id)
        {
            var response = await _userRepository.GetUserByIdAsync(Id);
            return Ok(response);
        }
    }
}
