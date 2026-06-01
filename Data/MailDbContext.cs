using Microsoft.EntityFrameworkCore;
using e_paositra.Models;

namespace Data;

public class MailDbContext : DbContext
{
    public MailDbContext(DbContextOptions<MailDbContext> options) : base(options) {}
    public DbSet<Mail> Mails { get; set; }
    public DbSet<History> Histories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Vehicle> Vehicles{get; set; }
    
}