using System;
using Microsoft.EntityFrameworkCore;
using e_paositra.Models;

namespace Data;

public class MailDbContext : DbContext
{
    public MailDbContext(DbContextOptions<MailDbContext> options) : base(options) {}
    public DbSet<Mail> Mails { get; set; }
    public DbSet<MailType> MailTypes { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<MailStatus> MailStatuses { get; set; }
    public DbSet<History> Histories { get; set; }
    public DbSet<User> Users { get; set; }
}