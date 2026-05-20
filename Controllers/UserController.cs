using Data;
using Microsoft.AspNetCore.Mvc;
using ViewModel;
using e_paositra.Models;
using Repository;

namespace e_paositra.Controllers;
public class UserController : Controller
{
    private readonly MailDbContext _context;
    private readonly IUserRepository _userRepository;
    public UserController( IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public IActionResult login()
    {
        return View();
    }
    [HttpGet]
    public IActionResult register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> register(RegisterViewModel model)
    {
        if(!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Role = model.Role,
            ServiceId = model.ServiceId
        };
        await _userRepository.AddUserAsync(user);
        await _userRepository.SaveAsync();
        return RedirectToAction("login");
    }

    [HttpPost
    ]
    public async Task<IActionResult> login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userRepository.LoginAsync(model.Email, model.Password);
        
        if (user != null)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Invalid login attempt.");
        return View(model);
    }
}