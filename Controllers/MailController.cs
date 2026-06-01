using Microsoft.AspNetCore.Mvc;
using e_paositra.Models;
using ViewModel;
using e_paositra.Services;
using Repository;

namespace e_paositra.Controllers;

public class MailController : Controller
{
    private readonly IMailRepository _mailRepository;
    private readonly IHistoryRepository _historyRepository;
    private readonly IUserRepository _userRepository;
    private readonly AgencyService _agencyService;
    private readonly IVehicleRepository _vehicleRepository;

    public MailController(IMailRepository mailRepository, IHistoryRepository historyRepository, IUserRepository userRepository, AgencyService agencyService, IVehicleRepository vehicleRepository)
    {
        _mailRepository = mailRepository;
        _historyRepository = historyRepository;
        _userRepository = userRepository;
        _agencyService = agencyService;
        _vehicleRepository = vehicleRepository;
    }

    // INDEX
    public async Task<IActionResult> Index()
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrWhiteSpace(userEmail)) return RedirectToAction("Login", "User");
        var mails = (await _mailRepository.GetMailsByUserEmailAsync(userEmail)).ToList();
        await AutoUpdateMailStatuses(mails);
        var mailIds = mails.Select(m => m.Id);
        var inboxMails = mails.Where(m => m.Recipient == userEmail).ToList();
        var outboxMails = mails.Where(m => m.Sender == userEmail).ToList();
        var viewModel = new MailIndexViewModel
        {
            CurrentUserEmail = userEmail,
            InboxCount = inboxMails.Count,
            OutboxCount = outboxMails.Count,
            PendingCount = mails.Count(m => m.Status == "En attente"),
            SentCount = mails.Count(m => m.Status == "Envoyé"),
            DeliveredCount = mails.Count(m => m.Status == "Livré"),
            InboxMails = inboxMails.Select(m => ToSummary(m)),
            OutboxMails = outboxMails.Select(m => ToSummary(m))
        };

        return View(viewModel);
    }
    // DETAILS
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var mail = await _mailRepository.GetMailByIdAsync(id.Value);
        if (mail == null) return NotFound();
        await AutoUpdateMailStatuses(new List<Mail> { mail });
        ViewBag.History = await _historyRepository.GetHistoriesByMailIdAsync(mail.Id);
        return View(mail);
    }
    // CREATE
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrWhiteSpace(userEmail))
            return RedirectToAction("Login", "User");

        var allVehicles = await _vehicleRepository.GetAllVehicleAsync();
        var availableVehicles = allVehicles
            .Where(v => v != null && v.State != "En maintenance")
            .ToList();

        ViewBag.Agencies = _agencyService.GetAllAgencies();
        ViewBag.Vehicles = availableVehicles;

        return View(new Mail { Sender = userEmail });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Mail mail)
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrWhiteSpace(userEmail))
            return RedirectToAction("Login", "User");

        mail.Sender = userEmail;
        mail.Recipient = $"{mail.PostalCode} - {mail.Address}";

        ModelState.Clear();
        TryValidateModel(mail);

        if (!ModelState.IsValid) return View(mail);

        mail.DateSent = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(mail.Duration))
        {
            int hours = 0, minutes = 0;
            foreach (var p in mail.Duration.Split(' '))
            {
                if (p.EndsWith("h")) int.TryParse(p.Replace("h", ""), out hours);
                if (p.EndsWith("m")) int.TryParse(p.Replace("m", ""), out minutes);
            }
            mail.DateReceived = mail.DateSent.AddHours(hours).AddMinutes(minutes);
        }

        await _mailRepository.AddMailAsync(mail);
        await _mailRepository.SaveAsync();
        mail.Status = "En attente";

        await _historyRepository.AddHistoryAsync(new History
        {
            MailId = mail.Id,
            ActionDate = DateTime.UtcNow,
            Action = "En attente",
            Comment = "Courrier créé et prêt à être envoyé.",
            UserId = HttpContext.Session.GetInt32("UserId") ?? 0
        });

        await _historyRepository.SaveAsync();

        TempData["JustSent"] = true;
        return RedirectToAction(nameof(Details), new { id = mail.Id });
    }

    // DELETE
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var mail = await _mailRepository.GetMailByIdAsync(id.Value);
        if (mail == null) return NotFound();

        return View(mail);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var mail = await _mailRepository.GetMailByIdAsync(id);
        if (mail == null) return NotFound();

        await _historyRepository.DeleteHistoriesByMailIdAsync(id);
        await _mailRepository.DeleteMailAsync(id);
        await _mailRepository.SaveAsync();

        return RedirectToAction(nameof(Dashboard));
    }

    // DASHBOARD
    public async Task<IActionResult> Dashboard()
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrWhiteSpace(userEmail))
            return RedirectToAction("Login", "User");

        var user = await _userRepository.GetUserByEmailAsync(userEmail);
        var mails = (await _mailRepository.GetMailsByUserEmailAsync(userEmail)).ToList();

        await AutoUpdateMailStatuses(mails);

        var mailIds = mails.Select(m => m.Id);

        var viewModel = new DashboardViewModel
        {
            CurrentUserEmail = userEmail,
            FirstName = user?.FirstName ?? userEmail.Split('@')[0],
            LastName = user?.LastName ?? "",
            InboxCount = mails.Count(m => m.Recipient == userEmail),
            OutboxCount = mails.Count(m => m.Sender == userEmail),
            PendingCount = mails.Count(m => m.Status == "En attente"),
            SentCount = mails.Count(m => m.Status == "Envoyé"),
            DeliveredCount = mails.Count(m => m.Status == "Livré"),
            RecentMails = mails.Take(5).Select(m => ToSummary(m))
        };

        return View(viewModel);
    }

    private async Task AutoUpdateMailStatuses(List<Mail> mails)
    {
        var now = DateTime.UtcNow;
        bool changed = false;

        foreach (var mail in mails)
        {
            var latestStatus = await _mailRepository.GetLatestMailAsync();
            var currentStatus = latestStatus?.Status ?? "En attente";

            if (now >= mail.DateReceived)
            {
                if (currentStatus == "Livré") continue;

                mail.Status = "Livré";

                await _historyRepository.AddHistoryAsync(new History
                {
                    MailId = mail.Id,
                    ActionDate = mail.DateReceived,
                    Action = "Livré",
                    Comment = "Courrier livré au destinataire.",
                    UserId = 0
                });
                changed = true;
            }
            else if (currentStatus == "En attente")
            {
                var totalDuration = mail.DateReceived - mail.DateSent;
                var delay = totalDuration * 0.1;
                if (now - mail.DateSent >= delay)
                {
                    mail.Status = "Envoyé";
                    await _historyRepository.AddHistoryAsync(new History
                    {
                        MailId = mail.Id,
                        ActionDate = mail.DateSent.Add(delay),
                        Action = "Envoyé",
                        Comment = "Courrier envoyé.",
                        UserId = 0
                    });
                    changed = true;
                }
            }
        }

        if (changed)
        {
            await _historyRepository.SaveAsync();
            await _mailRepository.SaveAsync();
        }
    }
    private static Dictionary<int, string> BuildLatestStatusDictionary(IEnumerable<Mail> statuses)
    {
        return statuses
            .GroupBy(ms => ms.Id)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(ms => ms.Id).First().Status ?? "En attente"
            );
    }
    private static MailSummaryViewModel ToSummary(Mail m) => new()
    {
        Id = m.Id,
        Reference = m.Reference,
        Sender = m.Sender,
        Recipient = m.Recipient,
        DateSent = m.DateSent,
        StatusLabel = m.Status ?? "En attente",
    };

    [HttpPost]
    public async Task<IActionResult> CalculateDistance(string startAgency, string endAgency)
    {
        if (string.IsNullOrWhiteSpace(startAgency) || string.IsNullOrWhiteSpace(endAgency))
            return Json(new { success = false, message = "Veuillez sélectionner les agences de départ et d'arrivée." });

        var agencies = _agencyService.GetAllAgencies();
        var start = agencies.FirstOrDefault(a => a.Name == startAgency);
        var end = agencies.FirstOrDefault(a => a.Name == endAgency);

        if (start == null || end == null)
            return Json(new { success = false, message = "Une ou plusieurs agences sélectionnées sont introuvables." });

        string startCoords = $"{start.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{start.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
        string endCoords = $"{end.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{end.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
        string apiKey = "eyJvcmciOiI1YjNjZTM1OTc4NTExMTAwMDFjZjYyNDgiLCJpZCI6IjJiYjgzZjUyNWQwMjRlNDc5MzhmNDNlYWEzZTUxZjMxIiwiaCI6Im11cm11cjY0In0=";
        string url = $"https://api.openrouteservice.org/v2/directions/driving-car?api_key={apiKey}&start={startCoords}&end={endCoords}";
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "e-paositra-distance-calculator");

        try
        {
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, message = "Erreur API OpenRouteService ou problème de connexion." });

            var jsonString = await response.Content.ReadAsStringAsync();
            using var doc = System.Text.Json.JsonDocument.Parse(jsonString);

            var features = doc.RootElement.GetProperty("features");
            if (features.GetArrayLength() == 0)
                return Json(new { success = false, message = "Aucun itinéraire trouvé entre ces deux agences." });

            var summary = features[0].GetProperty("properties").GetProperty("summary");

            double distanceInKm = Math.Round(summary.GetProperty("distance").GetDouble() / 1000.0, 2);

            double totalHours = distanceInKm / 10.0;
            var timeSpan = TimeSpan.FromHours(totalHours);

            string duration = timeSpan.TotalHours >= 1
                ? $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m"
                : $"{timeSpan.Minutes}m";

            return Json(new { success = true, distance = distanceInKm, duration });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Une erreur est survenue : {ex.Message}" });
        }
    }
}