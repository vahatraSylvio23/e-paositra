namespace e_paositra.Models;

public class Mail
{
    public int Id { get; set; }
    public string? reference { get; set; }
    public string? sender { get; set; }
    public string? recipient { get; set; }
    public int MailtypeId { get; set; }
    public int MailStatusId { get; set; }
    public DateTime DateSent { get; set; }
    public DateTime DateReceived { get; set; } 
    public int ServiceId { get; set; }
    public string? Observation {get; set; }
}