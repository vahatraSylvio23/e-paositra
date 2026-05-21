using Data;
using Microsoft.EntityFrameworkCore;
using e_paositra.Models;
namespace Repository;

public class ServiceRepository : IServiceRepository
{
    private readonly MailDbContext _context;
    public ServiceRepository(MailDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Service>> GetAllServicesAsync()
    {
        return await _context.Services.ToListAsync();
    }
    public async Task<Service?> GetServiceByIdAsync(int id)
    {
        return await _context.Services.FindAsync(id);
    }
    public Task AddServiceAsync(Service service)
    {
        _context.Services.Add(service);
        return Task.CompletedTask;
    }
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
    public void UpdateService(Service service)
    {
        _context.Services.Update(service);
    }
    public void DeleteService(Service service)
    {
        _context.Services.Remove(service);
    }


}