using e_paositra.Models;

namespace Repository;

public interface IMailStatusRepository
{
    Task<IEnumerable<MailStatus>> GetMailStatusesAsync(int mailId);
    Task<IEnumerable<MailStatus>> GetMailStatusesByMailIdsAsync(IEnumerable<int> mailIds);
    Task<MailStatus?> GetLatestMailStatusAsync(int mailId);
    Task<MailStatus?> GetMailStatusesByIdAsync(int id);
    Task AddMailStatusAsync(MailStatus mailStatus);
    void UpdateMailStatus(MailStatus mailStatus);
    void DeleteMailStatus(MailStatus mailStatus);
    Task DeleteMailStatusesByMailIdAsync(int mailId);
    Task SaveAsync();
}