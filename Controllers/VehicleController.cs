using Microsoft.AspNetCore.Mvc;
using Repository;
using ViewModel;
using e_paositra.Models;

namespace e_paositra.Controllers;

public class VehicleController : Controller
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMailRepository _mailRepository;

    public VehicleController(IVehicleRepository vehicleRepository, IMailRepository mailRepository)
    {
        _vehicleRepository = vehicleRepository;
        _mailRepository = mailRepository;
    }

    private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";
    private bool IsLoggedIn() => !string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail"));

    public async Task<IActionResult> Index()
    {
        if (!IsLoggedIn()) return RedirectToAction("Login", "User");
        if (!IsAdmin())    return RedirectToAction("Dashboard", "Mail");

        var vehicles = (await _vehicleRepository.GetAllVehicleAsync()).ToList();

        foreach (var v in vehicles)
        {
            if(v != null)
            await SyncVehicleStateAsync(v);
        }

        return View(vehicles);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        if (!IsLoggedIn()) return RedirectToAction("Login", "User");
        if (!IsAdmin())    return RedirectToAction("Dashboard", "Mail");

        var vehicle = await _vehicleRepository.GetVehicleByIdAsync(id.Value);
        if (vehicle == null) return NotFound();

        var mails = (await _mailRepository.GetMailsByVehicleId(vehicle.Id)).ToList();
        await SyncVehicleStateAsync(vehicle, mails);

        var viewModel = new VehicleViewModel
        {
            VehicleLicensePlate = vehicle.LicensePlate,
            MailSent            = mails,
            VehicleState        = vehicle.State ?? "Disponible",
            VehicleDriver       = vehicle.Driver ?? "N/A",
            VehicleLocation     = vehicle.Location ?? "N/A",
            VehicleLeft         = vehicle.Left,
            VehicleArrived      = vehicle.Arrived
        };

        ViewBag.VehicleId = vehicle.Id;
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsLoggedIn()) return RedirectToAction("Login", "User");
        if (!IsAdmin())    return RedirectToAction("Dashboard", "Mail");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vehicle vehicle)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login", "User");
        if (!IsAdmin())    return RedirectToAction("Dashboard", "Mail");
        if (!ModelState.IsValid) return View(vehicle);

        vehicle.State   = "Disponible";
        vehicle.Left    = DateTime.MinValue;
        vehicle.Arrived = DateTime.MinValue;

        await _vehicleRepository.AddVehicleAsync(vehicle);
        await _vehicleRepository.SaveAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        if (!IsLoggedIn()) return RedirectToAction("Login", "User");
        if (!IsAdmin())    return RedirectToAction("Dashboard", "Mail");

        var vehicle = await _vehicleRepository.GetVehicleByIdAsync(id.Value);
        if (vehicle == null) return NotFound();

        return View(vehicle);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Vehicle vehicle)
    {
        if (id != vehicle.Id) return NotFound();
        if (!IsLoggedIn()) return RedirectToAction("Login", "User");
        if (!IsAdmin())    return RedirectToAction("Dashboard", "Mail");

        if (!ModelState.IsValid) return View(vehicle);

        var existing = await _vehicleRepository.GetVehicleByIdAsync(id);
        if (existing == null) return NotFound();

        existing.LicensePlate = vehicle.LicensePlate;
        existing.Driver       = vehicle.Driver;

        await _vehicleRepository.UpdateVehicleAsync(existing);
        await _vehicleRepository.SaveAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        if (!IsLoggedIn()) return RedirectToAction("Login", "User");
        if (!IsAdmin())    return RedirectToAction("Dashboard", "Mail");

        var vehicle = await _vehicleRepository.GetVehicleByIdAsync(id.Value);
        if (vehicle == null) return NotFound();

        var mails = (await _mailRepository.GetMailsByVehicleId(vehicle.Id)).ToList();
        await SyncVehicleStateAsync(vehicle, mails);

        var viewModel = new VehicleViewModel
        {
            VehicleLicensePlate = vehicle.LicensePlate,
            MailSent            = mails,
            VehicleState        = vehicle.State ?? "Disponible",
            VehicleDriver       = vehicle.Driver ?? "N/A",
            VehicleLocation     = vehicle.Location ?? "N/A",
            VehicleLeft         = vehicle.Left,
            VehicleArrived      = vehicle.Arrived
        };

        ViewBag.VehicleId = vehicle.Id;
        return View(viewModel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!IsLoggedIn()) return RedirectToAction("Login", "User");
        if (!IsAdmin())    return RedirectToAction("Dashboard", "Mail");

        var vehicle = await _vehicleRepository.GetVehicleByIdAsync(id);
        if (vehicle == null) return NotFound();

        var mails = (await _mailRepository.GetMailsByVehicleId(id)).ToList();
        foreach (var mail in mails)
        {
            mail.VehicleId = 0;
            await _mailRepository.UpdateMailAsync(mail);
        }
        await _mailRepository.SaveAsync();

        await _vehicleRepository.DeleteVehicleAsync(id);
        await _vehicleRepository.SaveAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task SyncVehicleStateAsync(Vehicle vehicle, List<Mail>? mails = null)
    {
        mails ??= (await _mailRepository.GetMailsByVehicleId(vehicle.Id)).ToList();

        var activeMails = mails.Where(m => m.Status != "Livré").ToList();

        bool changed = false;

        string newState = activeMails.Any() ? "Indisponible" : "Disponible";
        if (vehicle.State != newState) { vehicle.State = newState; changed = true; }

        if (activeMails.Any())
        {
            var newLeft    = activeMails.Min(m => m.DateSent);
            var newArrived = activeMails.Max(m => m.DateReceived);

            if (vehicle.Left != newLeft)       { vehicle.Left    = newLeft;    changed = true; }
            if (vehicle.Arrived != newArrived) { vehicle.Arrived = newArrived; changed = true; }
        }
        else
        {
            if (vehicle.Left != DateTime.MinValue)    { vehicle.Left    = DateTime.MinValue; changed = true; }
            if (vehicle.Arrived != DateTime.MinValue) { vehicle.Arrived = DateTime.MinValue; changed = true; }
        }

        if (changed)
        {
            await _vehicleRepository.UpdateVehicleAsync(vehicle);
            await _vehicleRepository.SaveAsync();
        }
    }
}