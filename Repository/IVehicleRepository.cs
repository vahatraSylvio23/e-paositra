using e_paositra.Models;
namespace Repository;

public interface IVehicleRepository
{
    Task<IEnumerable<Vehicle?>> GetAllVehicleAsync();
    Task<Vehicle?> UpdateVehicleAsync(Vehicle vehicle);
    Task<Vehicle?> GetVehicleByIdAsync(int id);
    Task AddVehicleAsync(Vehicle vehicle);
    Task DeleteVehicleAsync(int id);
    Task SaveAsync();
}