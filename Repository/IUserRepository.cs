using e_paositra.Models;

namespace Repository;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task SaveAsync();
    Task <User?>LoginAsync(string email, string password);
}