using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeleutDictionary.API.Services;

namespace TeleutDictionary.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class HistoryController : ControllerBase
    {
        private readonly HistoryService _historyService;

        public HistoryController(HistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory()
        {
            var userId = GetUserId();
            var history = await _historyService.GetUserHistoryAsync(userId);
            return Ok(new { data = history });
        }

        [HttpDelete]
        public async Task<IActionResult> ClearHistory()
        {
            var userId = GetUserId();
            var result = await _historyService.ClearUserHistoryAsync(userId);

            if (!result)
                return BadRequest(new { message = "Ошибка при очистке истории" });

            return Ok(new { message = "История очищена" });
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}