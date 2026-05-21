namespace e_paositra.Models;

public class MailStatus
{
    public int Id { get; set; }
    public string? status { get; set; }
    public string? Type { get; set; }
    public int MailId { get; set; }
    public Mail? Mail { get; set; }
}