namespace e_paositra.Models;

public class Mail
{
    public int Id { get; set; }
    public string? Reference { get; set; }
    public string? Sender { get; set; }
    public string? Recipient { get; set; }
    public string? PostalCode { get; set; }
    public string? Address { get; set; }
    public string? Type {get; set;}
    public string? Status { get; set; }
    public DateTime DateSent { get; set; }
    public DateTime DateReceived { get; set; }
    public string? StartAgency { get; set; }
    public string? EndAgency { get; set; }
    public double? Distance { get; set; }
    public string? Duration { get; set; }
    public int VehicleId{get; set; }
    public string? Observation { get; set; }
}