
namespace ViewModel;

public class MailSummaryViewModel
{
    public int Id { get; set; }
    public string? Reference { get; set; }
    public string? Sender { get; set; }
    public string? Recipient { get; set; }
    public string? MailTypeName { get; set; }
    public DateTime DateSent { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public string MailPreview { get; set; } = string.Empty;
}

public class MailIndexViewModel
{
    public IEnumerable<MailSummaryViewModel> InboxMails { get; set; } = Array.Empty<MailSummaryViewModel>();
    public IEnumerable<MailSummaryViewModel> OutboxMails { get; set; } = Array.Empty<MailSummaryViewModel>();
    public int TotalCount { get; set; }
    public int InboxCount { get; set; }
    public int OutboxCount { get; set; }
    public int PendingCount { get; set; }
    public int SentCount { get; set; }
    public int DeliveredCount { get; set; }
    public string CurrentUserEmail { get; set; } = string.Empty;
}

public class DashboardViewModel
{
    public string CurrentUserEmail { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName {get ; set; } = string.Empty;

    public int InboxCount { get; set; }
    public int OutboxCount { get; set; }
    public int PendingCount { get; set; }
    public int SentCount { get; set; }
    public int DeliveredCount { get; set; }

    public IEnumerable<MailSummaryViewModel> RecentMails { get; set; } = Array.Empty<MailSummaryViewModel>();
}