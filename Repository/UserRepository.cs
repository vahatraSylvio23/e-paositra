using e_paositra.Models;
using Microsoft.EntityFrameworkCore;
using Data;
namespace Repository;

public class UserRepository : IUserRepository
{
    private readonly MailDbContext _context;

    public UserRepository(MailDbContext context)
    {
        _context = context;
    }
    public Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        return Task.CompletedTask;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return user;
        }
        return null;
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}