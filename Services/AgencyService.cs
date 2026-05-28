using System.Text.Json;
using e_paositra.Models;

namespace e_paositra.Services;

public class AgencyService
{
    private readonly string _jsonPath;

    public AgencyService(string jsonPath)
    {
        _jsonPath = jsonPath;
    }

    public List<Agency> GetAllAgencies()
    {
        if (!File.Exists(_jsonPath)) return new List<Agency>();

        try
        {
            var content = File.ReadAllText(_jsonPath);
            var agencies = JsonSerializer.Deserialize<List<Agency>>(content);
            return agencies?.OrderBy(a => a.Name).ToList() ?? new List<Agency>();
        }
        catch
        {
            return new List<Agency>();
        }
    }
}
