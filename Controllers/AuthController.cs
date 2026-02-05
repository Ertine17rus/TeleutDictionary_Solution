using Microsoft.AspNetCore.Mvc;
using TeleutDictionary.API.Models.DTOs;
using TeleutDictionary.API.Services;

namespace TeleutDictionary.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.RegisterAsync(registerDto);

            if (user == null)
                return BadRequest(new { message = "Пользователь с таким именем уже существует" });

            var token = _authService.GenerateJwtToken(user);

            return Ok(new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Token = token,
                TokenExpiry = DateTime.UtcNow.AddMinutes(60)
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.AuthenticateAsync(loginDto);

            if (user == null)
                return Unauthorized(new { message = "Неверное имя пользователя или пароль" });

            var token = _authService.GenerateJwtToken(user);

            return Ok(new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Token = token,
                TokenExpiry = DateTime.UtcNow.AddMinutes(60)
            });
        }
    }
}