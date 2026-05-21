using e_paositra.Models;

namespace Repository;

public interface IHistoryRepository
{
    Task<IEnumerable<History>> GetAllSync(int mailId);
    Task<History?> GetHistoryByIdAsync(int id);
    Task AddHistoryAsync(History history);
    Task SaveAsync();
    void UpdateHistory(History history);
    void deleteHistory(History history);
}