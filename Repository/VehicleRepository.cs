using Data;
using e_paositra.Models;
using Microsoft.EntityFrameworkCore;
namespace Repository;

public class VehicleRepository : IVehicleRepository
{
    private readonly MailDbContext _context;
    public VehicleRepository(MailDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Vehicle?>> GetAllVehicleAsync()
    {
        return await _context.Vehicles.ToListAsync();
    }
    public async Task<Vehicle?> GetVehicleByIdAsync(int id)
    {
        return await _context.Vehicles.FindAsync(id);
    }
    public async Task AddVehicleAsync(Vehicle vehicle)
    {
        await _context.Vehicles.AddAsync(vehicle);
    }
    public async Task DeleteVehicleAsync(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null)
            _context.Vehicles.Remove(vehicle);
    }
    public async Task<Vehicle?> UpdateVehicleAsync(Vehicle vehicle)
    {
        await _context.Vehicles.FindAsync(vehicle.Id);
        _context.Vehicles.Update(vehicle);
        return vehicle;
    }
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();       
    }
}