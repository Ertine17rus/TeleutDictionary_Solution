using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeleutDictionary.API.Data;
using TeleutDictionary.API.Models;
using TeleutDictionary.API.Services;

namespace TeleutDictionary.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class SeedController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HistoryService _historyService;

        public SeedController(AppDbContext context, HistoryService historyService)
        {
            _context = context;
            _historyService = historyService;
        }

        [HttpPost("teleut-words")]
        public async Task<IActionResult> SeedTeleutWords()
        {
            var userId = GetUserId();

            var existingWords = await _context.Words
                .Where(w => w.UserId == userId)
                .CountAsync();

            if (existingWords > 0)
                return BadRequest(new { message = "У вас уже есть слова в словаре" });

            var teleutWords = new List<Word>
            {
                new Word { Russian = "аба", Teleut = "отец", PartOfSpeech = "сущ.", UserId = userId },
                new Word { Russian = "абагай", Teleut = "брат отца (старший)", PartOfSpeech = "сущ.", UserId = userId },
                new Word { Russian = "агаш", Teleut = "дерево, лес", PartOfSpeech = "сущ.", UserId = userId },
                new Word { Russian = "ай", Teleut = "месяц, луна", PartOfSpeech = "сущ.", UserId = userId },
                new Word { Russian = "айак", Teleut = "нога", PartOfSpeech = "сущ.", UserId = userId },
                new Word { Russian = "айу", Teleut = "медведь", PartOfSpeech = "сущ.", UserId = userId },
                new Word { Russian = "алабуга", Teleut = "окунь", PartOfSpeech = "сущ.", UserId = userId },
                new Word { Russian = "алас", Teleut = "заклинание", PartOfSpeech = "сущ.", UserId = userId },
                new Word { Russian = "ана", Teleut = "мать", PartOfSpeech = "сущ.", UserId = userId },
                new Word { Russian = "ач", Teleut = "голодный", PartOfSpeech = "прил.", UserId = userId },
            };

            foreach (var word in teleutWords)
            {
                word.CreatedAt = DateTime.UtcNow;
                word.UpdatedAt = DateTime.UtcNow;
            }

            await _context.Words.AddRangeAsync(teleutWords);
            await _context.SaveChangesAsync();

            await _historyService.AddRecordAsync(userId, "SEED_WORDS",
                $"Загружено {teleutWords.Count} телеутских слов");

            return Ok(new
            {
                message = $"Добавлено {teleutWords.Count} телеутских слов",
                count = teleutWords.Count
            });
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}