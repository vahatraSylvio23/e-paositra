namespace e_paositra.Models;

public class Vehicle
{
    public int Id {get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public int MailId { get; set; }
    public string? State{ get; set; }
    public string? Driver { get; set; }
    public string? Location { get; set; }
    public DateTime Left { get; set; }
    public DateTime Arrived { get; set; }
}