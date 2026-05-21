using System.Threading.Tasks;
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
    public Task AddHistoryAsync(History history)
    {
        _context.Histories.Add(history);
        return Task.CompletedTask;
    }
    public async Task<IEnumerable<History>> GetAllSync(int mailId)
    {
        return await _context.Histories.Where(h => h.MailId == mailId).ToListAsync();
    }
    public async Task<History?> GetHistoryByIdAsync(int id)
    {
        return await _context.Histories.FindAsync(id);
    }
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
    public void UpdateHistory(History history)
    {
        _context.Histories.Update(history);
    }
    public void deleteHistory(History history)
    {
        _context.Histories.Remove(history);
    }
}