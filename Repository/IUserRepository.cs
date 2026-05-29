using e_paositra.Models;

namespace Repository;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task<User?> LoginAsync(string email, string password);
    Task SaveAsync();
}