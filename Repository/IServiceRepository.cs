using e_paositra.Models;
namespace Repository;

public interface IServiceRepository
{
    
    Task<IEnumerable<Service>> GetAllServicesAsync();
    Task<Service?> GetServiceByIdAsync(int id);
    Task AddServiceAsync(Service service);
    Task SaveAsync();
    void UpdateService(Service service);
    void DeleteService(Service service);
}