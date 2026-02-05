using TeleutDictionary.API.Data;
using TeleutDictionary.API.Models;
using TeleutDictionary.API.Models.DTOs;

using Microsoft.EntityFrameworkCore;

namespace TeleutDictionary.API.Services
{
    public class HistoryService
    {
        private readonly AppDbContext _context;

        public HistoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRecordAsync(int userId, string action, string details)
        {
            var record = new HistoryRecord
            {
                UserId = userId,
                Action = action,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _context.HistoryRecords.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task<List<HistoryRecordDto>> GetUserHistoryAsync(int userId, int limit = 50)
        {
            var records = await _context.HistoryRecords
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.Timestamp)
                .Take(limit)
                .ToListAsync();

            return records.Select(r => new HistoryRecordDto
            {
                Id = r.Id,
                Action = r.Action,
                Details = r.Details,
                Timestamp = r.Timestamp
            }).ToList();
        }

        public async Task<bool> ClearUserHistoryAsync(int userId)
        {
            var records = _context.HistoryRecords.Where(h => h.UserId == userId);
            _context.HistoryRecords.RemoveRange(records);

            await AddRecordAsync(userId, "CLEAR_HISTORY", "Очищена история действий");

            return await _context.SaveChangesAsync() > 0;
        }
    }
}