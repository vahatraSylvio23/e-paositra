using e_paositra.Models;

namespace ViewModel;


public class VehicleViewModel
{
    public string VehicleLicensePlate { get; set; } = string.Empty;
    public IEnumerable<User>? User { get; set; }
    public IEnumerable<Mail>? MailSent { get; set; }
    public string? VehicleState{ get; set; }
    public string? VehicleDriver { get; set; }
    public string? VehicleLocation { get; set; }
    public DateTime VehicleLeft{get; set; }
    public DateTime VehicleArrived{get; set; }
}