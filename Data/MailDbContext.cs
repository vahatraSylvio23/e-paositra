using Microsoft.EntityFrameworkCore;

namespace Data;
public class MailDbContext : DbContext
{
    public MailDbContext(DbContextOptions<MailDbContext> options) : base(options)
    {}
    public DbSet<e_paositra.Models.Mail> Mails{get; set; }
    public DbSet<e_paositra.Models.Service> Services{get; set; }
    public DbSet<e_paositra.Models.MailStatus> MailStatuses{get; set; }
    public DbSet<e_paositra.Models.History> Histories{get; set; }
    public DbSet<e_paositra.Models.User> Users{get; set; }
}