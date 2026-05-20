using Microsoft.AspNetCore.Mvc;

namespace e_paositra.Controllers;

public class MailController :Controller
{
    public IActionResult Index()
    {
        return View();
    }
}