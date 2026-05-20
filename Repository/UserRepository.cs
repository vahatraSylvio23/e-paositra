using Data;
using e_paositra.Models;

namespace Repository;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly MailDbContext _context;

    public UserRepository(MailDbContext context)
    {
        _context = context;
    }
    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.
        FirstOrDefaultAsync(u => u.Email == email);
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
}