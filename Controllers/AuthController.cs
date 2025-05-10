using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using backendcafe.DTO;
using backendcafe.Services;

namespace MyApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDto)
        {
            if (await _authService.UserExists(registerDto.Username))
                return BadRequest("Username sudah digunakan");

            var user = await _authService.Register(registerDto);
            
            return user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto)
        {
            var user = await _authService.Login(loginDto);

            if (user == null) return Unauthorized("Username atau password salah");

            return user;
        }
    }
}