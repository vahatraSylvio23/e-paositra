using Data;
using e_paositra.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class UserRepository : IUserRepository
{
    private readonly MailDbContext _context;

    public UserRepository(MailDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        return Task.CompletedTask;
    }

    public Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || string.IsNullOrWhiteSpace(user.Password))
            return null;

        bool isBcrypt = user.Password.StartsWith("$2a$") || user.Password.StartsWith("$2b$");

        bool passwordValid = isBcrypt
            ? BCrypt.Net.BCrypt.Verify(password, user.Password)
            : user.Password == password; 

        if (passwordValid && !isBcrypt)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(password);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        return passwordValid ? user : null;
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}