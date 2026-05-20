namespace e_paositra.Models;

public class History
{
    public int Id { get; set; }
    public int MailId { get; set; }
    public DateTime ActionDate { get; set; }
    public string? Action { get; set; }
    public string? Comment { get; set; }
    public int UserId { get; set; }
}