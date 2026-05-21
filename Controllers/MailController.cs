using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using e_paositra.Models;
using ViewModel;

namespace e_paositra.Controllers;

public class MailController : Controller
{
    private readonly MailDbContext _context;

    public MailController(MailDbContext context)
    {
        _context = context;
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

        return View(new Mail { Sender = userEmail });
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
        mail.DateReceived = mail.MailTypeId switch
        {
            4 => mail.DateSent.AddDays(1),
            3 => mail.DateSent.AddDays(2),
            2 => mail.DateSent.AddDays(3),
            _ => mail.DateSent.AddDays(5)
        };

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
        return RedirectToAction(nameof(Index));
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
            else if (now - mail.DateSent >= TimeSpan.FromMinutes(5) && currentStatus == "En attente")
            {
                mail.MailStatusId = 2;
                _context.MailStatuses.Add(new MailStatus { status = "Envoyé", Type = "Automatique", MailId = mail.Id });
                _context.Histories.Add(new History
                {
                    MailId     = mail.Id,
                    ActionDate = mail.DateSent.AddMinutes(5),
                    Action     = "Envoyé",
                    Comment    = "Courrier envoyé automatiquement.",
                    UserId     = 0
                });
                changed = true;
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
}