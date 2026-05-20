using e_paositra.Models;
namespace Repository;
public interface IMailRepository
{
    Task AddMailAsync(Mail mail);
    
    Task<Mail?> GetMailByIdAsync(int id);
    Task SaveAsync();
}