using System;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using e_paositra.Models;
using ViewModel;

using e_paositra.Services;

namespace e_paositra.Controllers;

public class MailController : Controller
{
    private readonly MailDbContext _context;
    private readonly AgencyService _agencyService;

    public MailController(MailDbContext context, AgencyService agencyService)
    {
        _context = context;
        _agencyService = agencyService;
    }

    public async Task<IActionResult> Index()
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            return RedirectToAction("Login", "User");
        }

        // Une seule requête pour récupérer tous les courriers de l'utilisateur
        var mails = await _context.Mails
            .Where(m => m.Sender == userEmail || m.Recipient == userEmail)
            .OrderByDescending(m => m.DateSent)
            .ToListAsync();

        await AutoUpdateMailStatuses(mails);

        var mailIds = mails.Select(m => m.Id).ToList();

        var latestStatusDictionary = (await _context.MailStatuses
            .Where(ms => mailIds.Contains(ms.MailId))
            .ToListAsync())
            .GroupBy(ms => ms.MailId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(ms => ms.Id).First().status ?? "En attente"
            );

        var inboxMails  = mails.Where(m => m.Recipient == userEmail).ToList();
        var outboxMails = mails.Where(m => m.Sender   == userEmail).ToList();

        var viewModel = new MailIndexViewModel
        {
            CurrentUserEmail = userEmail,
            InboxCount    = inboxMails.Count,
            OutboxCount   = outboxMails.Count,
            PendingCount  = mails.Count(m => MapStatusLabel(latestStatusDictionary.GetValueOrDefault(m.Id)) == "En attente"),
            SentCount     = mails.Count(m => MapStatusLabel(latestStatusDictionary.GetValueOrDefault(m.Id)) == "Envoyé"),
            DeliveredCount= mails.Count(m => MapStatusLabel(latestStatusDictionary.GetValueOrDefault(m.Id)) == "Livré"),
            InboxMails = inboxMails.Select(m => new MailSummaryViewModel
            {
                Id          = m.Id,
                Reference   = m.Reference,
                Sender      = m.Sender,
                Recipient   = m.Recipient,
                DateSent    = m.DateSent,
                StatusLabel = MapStatusLabel(latestStatusDictionary.GetValueOrDefault(m.Id))
            }),
            OutboxMails = outboxMails.Select(m => new MailSummaryViewModel
            {
                Id          = m.Id,
                Reference   = m.Reference,
                Sender      = m.Sender,
                Recipient   = m.Recipient,
                DateSent    = m.DateSent,
                StatusLabel = MapStatusLabel(latestStatusDictionary.GetValueOrDefault(m.Id))
            })
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var mail = await _context.Mails.FindAsync(id);
        if (mail == null) return NotFound();

        await AutoUpdateMailStatuses(new List<Mail> { mail });

        var latestStatus = await _context.MailStatuses
            .Where(ms => ms.MailId == mail.Id)
            .OrderByDescending(ms => ms.Id)
            .FirstOrDefaultAsync();

        ViewBag.CurrentStatus = MapStatusLabel(latestStatus?.status);
        ViewBag.Statuses = await _context.MailStatuses.Where(ms => ms.MailId == mail.Id).ToListAsync();
        ViewBag.History  = await _context.Histories.Where(h => h.MailId == mail.Id).OrderByDescending(h => h.ActionDate).ToListAsync();

        return View(mail);
    }

    public IActionResult Create()
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            return RedirectToAction("Login", "User");
        }

        ViewBag.Agencies = _agencyService.GetAllAgencies();
        return View(new Mail { Sender = userEmail });
    }

    [HttpPost]
    public async Task<IActionResult> CalculateDistance(string startAgency, string endAgency)
    {
        if (string.IsNullOrWhiteSpace(startAgency) || string.IsNullOrWhiteSpace(endAgency))
        {
            return Json(new { success = false, message = "Veuillez sélectionner les agences de départ et d'arrivée." });
        }

        var agencies = _agencyService.GetAllAgencies();
        var start = agencies.FirstOrDefault(a => a.Name == startAgency);
        var end = agencies.FirstOrDefault(a => a.Name == endAgency);

        if (start == null || end == null)
        {
            return Json(new { success = false, message = "Une ou plusieurs agences sélectionnées sont introuvables." });
        }

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
            {
                return Json(new { success = false, message = "Erreur API OpenRouteService ou problème de connexion." });
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            using var doc = System.Text.Json.JsonDocument.Parse(jsonString);
            
            var features = doc.RootElement.GetProperty("features");
            if (features.GetArrayLength() == 0)
            {
                return Json(new { success = false, message = "Aucun itinéraire trouvé entre ces deux agences." });
            }

            var properties = features[0].GetProperty("properties");
            var summary = properties.GetProperty("summary");

            double distanceInMeters = summary.GetProperty("distance").GetDouble();
            double durationInSeconds = summary.GetProperty("duration").GetDouble();

            double distanceInKm = Math.Round(distanceInMeters / 1000.0, 2);
            
            var timeSpan = TimeSpan.FromSeconds(durationInSeconds);
            string durationFormatted = timeSpan.TotalHours >= 1 
                ? $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m" 
                : $"{timeSpan.Minutes}m";

            return Json(new { 
                success = true, 
                distance = distanceInKm, 
                duration = durationFormatted
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Une erreur est survenue : {ex.Message}" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Mail mail)
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            return RedirectToAction("Login", "User");
        }

        mail.Sender    = userEmail;
        mail.ServiceId = 1;
        mail.Recipient = $"{mail.PostalCode} - {mail.Address}";

        ModelState.Clear();
        TryValidateModel(mail);

        if (!ModelState.IsValid) return View(mail);

        mail.MailStatusId = 1;
        mail.DateSent     = DateTime.UtcNow;

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
        else
        {
            mail.DateReceived = mail.DateSent.AddDays(mail.MailTypeId switch { 4 => 1, 3 => 2, 2 => 3, _ => 5 });
        }

        _context.Mails.Add(mail);
        await _context.SaveChangesAsync();

        _context.MailStatuses.Add(new MailStatus
        {
            status = "En attente",
            Type   = "Automatique",
            MailId = mail.Id
        });

        _context.Histories.Add(new History
        {
            MailId     = mail.Id,
            ActionDate = DateTime.UtcNow,
            Action     = "En attente",
            Comment    = "Courrier créé et prêt à être envoyé.",
            UserId     = HttpContext.Session.GetInt32("UserId") ?? 0
        });

        await _context.SaveChangesAsync();
        TempData["JustSent"] = true;
        return RedirectToAction(nameof(Details), new { id = mail.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var mail = await _context.Mails.FindAsync(id);
        if (mail == null) return NotFound();

        return View(mail);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var mail = await _context.Mails.FindAsync(id);
        if (mail == null) return NotFound();

        _context.MailStatuses.RemoveRange(_context.MailStatuses.Where(ms => ms.MailId == id));
        _context.Histories.RemoveRange(_context.Histories.Where(h => h.MailId == id));
        _context.Mails.Remove(mail);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task AutoUpdateMailStatuses(List<Mail> mails)
    {
        var now     = DateTime.UtcNow;
        bool changed = false;

        foreach (var mail in mails)
        {
            var currentStatus = (await _context.MailStatuses
                .Where(ms => ms.MailId == mail.Id)
                .ToListAsync())
                .OrderByDescending(s => s.Id)
                .FirstOrDefault()?.status ?? "En attente";

            if (now >= mail.DateReceived)
            {
                if (currentStatus == "Livré") continue;

                mail.MailStatusId = 3;
                _context.MailStatuses.Add(new MailStatus { status = "Livré", Type = "Automatique", MailId = mail.Id });
                _context.Histories.Add(new History
                {
                    MailId     = mail.Id,
                    ActionDate = mail.DateReceived,
                    Action     = "Livré",
                    Comment    = "Courrier livré automatiquement au destinataire.",
                    UserId     = 0
                });
                changed = true;
            }
            else if (currentStatus == "En attente")
            {
                var totalDuration = mail.DateReceived - mail.DateSent;
                var delay = totalDuration * 0.1;
                if (now - mail.DateSent >= delay)
                {
                    mail.MailStatusId = 2;
                    _context.MailStatuses.Add(new MailStatus { status = "Envoyé", Type = "Automatique", MailId = mail.Id });
                    _context.Histories.Add(new History
                    {
                        MailId     = mail.Id,
                        ActionDate = mail.DateSent.Add(delay),
                        Action     = "Envoyé",
                        Comment    = "Courrier envoyé automatiquement.",
                        UserId     = 0
                    });
                    changed = true;
                }
            }
        }

        if (changed) await _context.SaveChangesAsync();
    }

    private static string MapStatusLabel(string? status) => status?.ToLowerInvariant() switch
    {
        var s when s?.Contains("livré")     == true => "Livré",
        var s when s?.Contains("envoyé")    == true => "Envoyé",
        var s when s?.Contains("en attente")== true => "En attente",
        null or "" => "En attente",
        _ => status!
    };
     
public async Task<IActionResult> Dashboard()
{
    var userEmail = HttpContext.Session.GetString("UserEmail");
    if (string.IsNullOrWhiteSpace(userEmail))
    {
        return RedirectToAction("Login", "User");
    }
 
    // Récupération de l'utilisateur pour le prénom
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
 
    // Tous les courriers de l'utilisateur
    var mails = await _context.Mails
        .Where(m => m.Sender == userEmail || m.Recipient == userEmail)
        .OrderByDescending(m => m.DateSent)
        .ToListAsync();
 
    await AutoUpdateMailStatuses(mails);
 
    var mailIds = mails.Select(m => m.Id).ToList();
 
    var latestStatusDictionary = (await _context.MailStatuses
        .Where(ms => mailIds.Contains(ms.MailId))
        .ToListAsync())
        .GroupBy(ms => ms.MailId)
        .ToDictionary(
            g => g.Key,
            g => g.OrderByDescending(ms => ms.Id).First().status ?? "En attente"
        );
 
    var viewModel = new DashboardViewModel
    {
        CurrentUserEmail = userEmail,
        FirstName        = user?.FirstName ?? userEmail.Split('@')[0],
        InboxCount       = mails.Count(m => m.Recipient == userEmail),
        OutboxCount      = mails.Count(m => m.Sender == userEmail),
        PendingCount     = mails.Count(m => MapStatusLabel(latestStatusDictionary.GetValueOrDefault(m.Id)) == "En attente"),
        SentCount        = mails.Count(m => MapStatusLabel(latestStatusDictionary.GetValueOrDefault(m.Id)) == "Envoyé"),
        DeliveredCount   = mails.Count(m => MapStatusLabel(latestStatusDictionary.GetValueOrDefault(m.Id)) == "Livré"),
        RecentMails      = mails.Take(5).Select(m => new MailSummaryViewModel
        {
            Id          = m.Id,
            Reference   = m.Reference,
            Sender      = m.Sender,
            Recipient   = m.Recipient,
            DateSent    = m.DateSent,
            StatusLabel = MapStatusLabel(latestStatusDictionary.GetValueOrDefault(m.Id))
        })
    };
 
    return View(viewModel);
}
 

}