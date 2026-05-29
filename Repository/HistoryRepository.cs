using Data;
using e_paositra.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class HistoryRepository : IHistoryRepository
{
    private readonly MailDbContext _context;

    public HistoryRepository(MailDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<History>> GetHistoriesByMailIdAsync(int mailId)
    {
        return await _context.Histories
            .Where(h => h.MailId == mailId)
            .OrderByDescending(h => h.ActionDate)
            .ToListAsync();
    }

    public Task AddHistoryAsync(History history)
    {
        _context.Histories.Add(history);
        return Task.CompletedTask;
    }

    public async Task DeleteHistoriesByMailIdAsync(int mailId)
    {
        var histories = await _context.Histories
            .Where(h => h.MailId == mailId)
            .ToListAsync();
        _context.Histories.RemoveRange(histories);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}