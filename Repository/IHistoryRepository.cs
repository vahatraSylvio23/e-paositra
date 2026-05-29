using e_paositra.Models;

namespace Repository;

public interface IHistoryRepository
{
    Task<IEnumerable<History>> GetHistoriesByMailIdAsync(int mailId);
    Task AddHistoryAsync(History history);
    Task DeleteHistoriesByMailIdAsync(int mailId);
    Task SaveAsync();
}