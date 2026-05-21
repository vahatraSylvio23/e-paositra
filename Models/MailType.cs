namespace e_paositra.Models;

public class MailType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal BasePrice { get; set; }
    public ICollection<Mail>? Mails { get; set; }
}