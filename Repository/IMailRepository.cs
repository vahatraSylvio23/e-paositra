using e_paositra.Models;

namespace Repository;

public interface IMailRepository
{
    Task AddMailAsync(Mail mail);
    Task<Mail?> GetMailByIdAsync(int id);
    Task<IEnumerable<Mail>> GetMailsByUserEmailAsync(string userEmail);
    Task<Mail?> GetLatestMailAsync();
    Task<IEnumerable<Mail>> GetMailsByVehicleId(int vehicleId);
    Task UpdateMailAsync(Mail mail);
    Task DeleteMailAsync(int id);
    Task SaveAsync();
}