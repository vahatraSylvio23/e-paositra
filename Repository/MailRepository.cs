using Data;
using e_paositra.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class MailRepository : IMailRepository
{
    private readonly MailDbContext _context;

    public MailRepository(MailDbContext context)
    {
        _context = context;
    }

    public Task AddMailAsync(Mail mail)
    {
        _context.Mails.Add(mail);
        return Task.CompletedTask;
    }

    public async Task<Mail?> GetMailByIdAsync(int id)
    {
        return await _context.Mails.FindAsync(id);
    }

    public async Task<IEnumerable<Mail>> GetMailsByUserEmailAsync(string userEmail)
    {
        return await _context.Mails
            .Where(m => m.Sender == userEmail || m.Recipient == userEmail)
            .OrderByDescending(m => m.DateSent)
            .ToListAsync();
    }

    public async Task<Mail?> GetLatestMailAsync()
    {
        return await _context.Mails.OrderByDescending(m => m.Id).FirstOrDefaultAsync();
    }
    public Task UpdateMailAsync(Mail mail)
    {
        _context.Mails.Update(mail);
        return Task.CompletedTask;
    }

    public async Task DeleteMailAsync(int id)
    {
        var mail = await _context.Mails.FindAsync(id);
        if (mail != null)
            _context.Mails.Remove(mail);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<Mail>> GetMailsByVehicleId(int vehicleId)
    {
        return await _context.Mails.Where(m => m.VehicleId == vehicleId).ToListAsync();
    }
}