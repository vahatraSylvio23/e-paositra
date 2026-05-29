using Data;
using e_paositra.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class MailStatusRepository : IMailStatusRepository
{
    private readonly MailDbContext _context;

    public MailStatusRepository(MailDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MailStatus>> GetMailStatusesAsync(int mailId)
    {
        return await _context.MailStatuses
            .Where(ms => ms.MailId == mailId)
            .ToListAsync();
    }

    public async Task<IEnumerable<MailStatus>> GetMailStatusesByMailIdsAsync(IEnumerable<int> mailIds)
    {
        return await _context.MailStatuses
            .Where(ms => mailIds.Contains(ms.MailId))
            .ToListAsync();
    }

    public async Task<MailStatus?> GetLatestMailStatusAsync(int mailId)
    {
        return await _context.MailStatuses
            .Where(ms => ms.MailId == mailId)
            .OrderByDescending(ms => ms.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<MailStatus?> GetMailStatusesByIdAsync(int id)
    {
        return await _context.MailStatuses.FindAsync(id);
    }

    public Task AddMailStatusAsync(MailStatus mailStatus)
    {
        _context.MailStatuses.Add(mailStatus);
        return Task.CompletedTask;
    }

    public void UpdateMailStatus(MailStatus mailStatus)
    {
        _context.MailStatuses.Update(mailStatus);
    }

    public void DeleteMailStatus(MailStatus mailStatus)
    {
        _context.MailStatuses.Remove(mailStatus);
    }

    public async Task DeleteMailStatusesByMailIdAsync(int mailId)
    {
        var statuses = await _context.MailStatuses
            .Where(ms => ms.MailId == mailId)
            .ToListAsync();
        _context.MailStatuses.RemoveRange(statuses);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}