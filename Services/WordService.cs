using Microsoft.EntityFrameworkCore;
using TeleutDictionary.API.Data;
using TeleutDictionary.API.Models;
using TeleutDictionary.API.Models.DTOs;

namespace TeleutDictionary.API.Services
{
    public class WordService
    {
        private readonly AppDbContext _context;
        private readonly HistoryService _historyService;

        public WordService(AppDbContext context, HistoryService historyService)
        {
            _context = context;
            _historyService = historyService;
        }

        public async Task<List<WordDto>> GetUserWordsAsync(int userId)
        {
            var words = await _context.Words
                .Where(w => w.UserId == userId)
                .OrderBy(w => w.Russian)
                .ToListAsync();

            return words.Select(w => new WordDto
            {
                Id = w.Id,
                Russian = w.Russian,
                Teleut = w.Teleut,
                PartOfSpeech = w.PartOfSpeech,
                Example = w.Example,
                Transcription = w.Transcription,
                Description = w.Description,
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt
            }).ToList();
        }

        public async Task<WordDto?> GetWordByIdAsync(int wordId, int userId)
        {
            var word = await _context.Words
                .FirstOrDefaultAsync(w => w.Id == wordId && w.UserId == userId);

            if (word == null) return null;

            return new WordDto
            {
                Id = word.Id,
                Russian = word.Russian,
                Teleut = word.Teleut,
                PartOfSpeech = word.PartOfSpeech,
                Example = word.Example,
                Transcription = word.Transcription,
                Description = word.Description,
                CreatedAt = word.CreatedAt,
                UpdatedAt = word.UpdatedAt
            };
        }

        public async Task<List<WordDto>> SearchWordsAsync(int userId, string query)
        {
            var words = await _context.Words
                .Where(w => w.UserId == userId &&
                           (w.Russian.Contains(query) ||
                            w.Teleut.Contains(query) ||
                            (w.Description != null && w.Description.Contains(query))))
                .OrderBy(w => w.Russian)
                .ToListAsync();

            return words.Select(w => new WordDto
            {
                Id = w.Id,
                Russian = w.Russian,
                Teleut = w.Teleut,
                PartOfSpeech = w.PartOfSpeech,
                Example = w.Example,
                Transcription = w.Transcription,
                Description = w.Description,
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt
            }).ToList();
        }

        public async Task<WordDto?> CreateWordAsync(int userId, CreateWordDto createDto)
        {
            var word = new Word
            {
                Russian = createDto.Russian,
                Teleut = createDto.Teleut,
                PartOfSpeech = createDto.PartOfSpeech,
                Example = createDto.Example,
                Transcription = createDto.Transcription,
                Description = createDto.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Words.Add(word);
            await _context.SaveChangesAsync();

            await _historyService.AddRecordAsync(userId, "ADD_WORD",
                $"Добавлено слово: {createDto.Russian} ↔ {createDto.Teleut}");

            return new WordDto
            {
                Id = word.Id,
                Russian = word.Russian,
                Teleut = word.Teleut,
                PartOfSpeech = word.PartOfSpeech,
                Example = word.Example,
                Transcription = word.Transcription,
                Description = word.Description,
                CreatedAt = word.CreatedAt,
                UpdatedAt = word.UpdatedAt
            };
        }

        public async Task<WordDto?> UpdateWordAsync(int wordId, int userId, UpdateWordDto updateDto)
        {
            var word = await _context.Words
                .FirstOrDefaultAsync(w => w.Id == wordId && w.UserId == userId);

            if (word == null) return null;

            if (!string.IsNullOrEmpty(updateDto.Russian))
                word.Russian = updateDto.Russian;

            if (!string.IsNullOrEmpty(updateDto.Teleut))
                word.Teleut = updateDto.Teleut;

            if (updateDto.PartOfSpeech != null)
                word.PartOfSpeech = updateDto.PartOfSpeech;

            if (updateDto.Example != null)
                word.Example = updateDto.Example;

            if (updateDto.Transcription != null)
                word.Transcription = updateDto.Transcription;

            if (updateDto.Description != null)
                word.Description = updateDto.Description;

            word.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _historyService.AddRecordAsync(userId, "UPDATE_WORD",
                $"Обновлено слово ID {wordId}: {word.Russian}");

            return new WordDto
            {
                Id = word.Id,
                Russian = word.Russian,
                Teleut = word.Teleut,
                PartOfSpeech = word.PartOfSpeech,
                Example = word.Example,
                Transcription = word.Transcription,
                Description = word.Description,
                CreatedAt = word.CreatedAt,
                UpdatedAt = word.UpdatedAt
            };
        }

        public async Task<bool> DeleteWordAsync(int wordId, int userId)
        {
            var word = await _context.Words
                .FirstOrDefaultAsync(w => w.Id == wordId && w.UserId == userId);

            if (word == null) return false;

            _context.Words.Remove(word);
            await _context.SaveChangesAsync();

            await _historyService.AddRecordAsync(userId, "DELETE_WORD",
                $"Удалено слово: {word.Russian} ↔ {word.Teleut}");

            return true;
        }
    }
}