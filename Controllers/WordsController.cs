using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeleutDictionary.API.Models.DTOs;
using TeleutDictionary.API.Services;

namespace TeleutDictionary.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class WordsController : ControllerBase
    {
        private readonly WordService _wordService;

        public WordsController(WordService wordService)
        {
            _wordService = wordService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWords()
        {
            var userId = GetUserId();
            var words = await _wordService.GetUserWordsAsync(userId);
            return Ok(new { data = words });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchWords([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest(new { message = "Поисковый запрос не может быть пустым" });

            var userId = GetUserId();
            var words = await _wordService.SearchWordsAsync(userId, query);
            return Ok(new { data = words });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWord(int id)
        {
            var userId = GetUserId();
            var word = await _wordService.GetWordByIdAsync(id, userId);

            if (word == null)
                return NotFound(new { message = "Слово не найдено" });

            return Ok(new { data = word });
        }

        [HttpPost]
        public async Task<IActionResult> CreateWord([FromBody] CreateWordDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var word = await _wordService.CreateWordAsync(userId, createDto);

            if (word == null)
                return BadRequest(new { message = "Ошибка при создании слова" });

            return CreatedAtAction(nameof(GetWord), new { id = word.Id }, new { data = word });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWord(int id, [FromBody] UpdateWordDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var word = await _wordService.UpdateWordAsync(id, userId, updateDto);

            if (word == null)
                return NotFound(new { message = "Слово не найдено" });

            return Ok(new { data = word });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWord(int id)
        {
            var userId = GetUserId();
            var result = await _wordService.DeleteWordAsync(id, userId);

            if (!result)
                return NotFound(new { message = "Слово не найдено" });

            return Ok(new { message = "Слово успешно удалено" });
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}