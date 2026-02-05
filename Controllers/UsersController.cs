using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeleutDictionary.API.Services;

namespace TeleutDictionary.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly WordService _wordService;

        public UsersController(AuthService authService, WordService wordService)
        {
            _authService = authService;
            _wordService = wordService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "Пользователь не найден" });

            var words = await _wordService.GetUserWordsAsync(userId);

            return Ok(new
            {
                data = new
                {
                    user.Id,
                    user.Username,
                    user.CreatedAt,
                    WordCount = words.Count
                }
            });
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}