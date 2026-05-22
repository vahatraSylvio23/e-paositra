using Microsoft.AspNetCore.Mvc;
using ViewModel;
using e_paositra.Models;
using Repository;

namespace e_paositra.Controllers;
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Modify()
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");

    if (string.IsNullOrWhiteSpace(userEmail))
    {
        return RedirectToAction(nameof(Index), "Mail");
    }

    var user = await _userRepository.GetUserByEmailAsync(userEmail);

    if (user == null)
    {
        return RedirectToAction(nameof(Login));
    }

    var model = new ModifyViewModel
    {
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Password = user.Password,
        PhoneNumber = user.PhoneNumber
    };

    return View(model);
    }    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Modify(ModifyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userEmail = HttpContext.Session.GetString("UserEmail");

        if (string.IsNullOrWhiteSpace(userEmail))
        {
            return RedirectToAction(nameof(Index), "Mail");
        }

        var user = await _userRepository.GetUserByEmailAsync(userEmail);

        if (user == null)
        {
            return RedirectToAction(nameof(Login));
        }

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PhoneNumber = model.PhoneNumber;
        user.Password = model.Password;

        await _userRepository.UpdateUserAsync(user);

        return RedirectToAction(nameof(Modify));
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existingUser = await _userRepository.GetUserByEmailAsync(model.Email ?? string.Empty);
        if (existingUser != null)
        {
            ModelState.AddModelError(nameof(model.Email), "Cet email est déjà utilisé.");
            return View(model);
        }

        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password ?? string.Empty),
            ServiceId = 1
        };

        await _userRepository.AddUserAsync(user);
        await _userRepository.SaveAsync();

        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userRepository.LoginAsync(model.Email ?? string.Empty, model.Password ?? string.Empty);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            return View(model);
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserEmail", user.Email ?? string.Empty);
        HttpContext.Session.SetString("UserRole", user.Role ?? string.Empty);

        return RedirectToAction("Index", "Mail");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }
}