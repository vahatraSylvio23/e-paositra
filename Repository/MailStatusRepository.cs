using Data;
using e_paositra.Models;
using Microsoft.EntityFrameworkCore;
namespace Repository;

public class MailStatusRepository : IMailStatusRepository
{
    private readonly MailDbContext _context;
    public async Task<IEnumerable<MailStatus>> GetMailStatusesAsync(int mailId)
    {
        return await _context.MailStatuses.Where(ms => ms.MailId == mailId).ToListAsync();
    }

}