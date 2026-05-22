using Data;
using e_paositra.Models;

namespace Repository;
public class MailRepository : IMailRepository
{
    private readonly MailDbContext mailDbContext;

    public MailRepository(MailDbContext mailDbContext)
    {
        this.mailDbContext = mailDbContext;
    }
    public Task AddMailAsync(Mail mail)
    {
        mailDbContext.Mails.Add(mail);
        return Task.CompletedTask;
    }
    public async Task<Mail?> GetMailByIdAsync(int id)
    {
        return await mailDbContext.Mails.FindAsync(id);
    }
    public async Task SaveAsync()    {
        await mailDbContext.SaveChangesAsync();
    }
}