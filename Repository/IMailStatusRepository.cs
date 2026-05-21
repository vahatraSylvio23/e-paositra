using e_paositra.Models;

namespace Repository;

public interface IMailStatusRepository
{
    Task<IEnumerable<MailStatus>> GetMailStatusesAsync(int mailId);
    Task<MailStatus?> GetMailStatusesByIdAsync(int id);
    Task AddMailStatusAsync(MailStatus mailStatus);
    Task SaveAsync();
    void UpdateMailStatus(MailStatus mailStatus);
    void DeleteMailStatus(MailStatus mailStatus);
}