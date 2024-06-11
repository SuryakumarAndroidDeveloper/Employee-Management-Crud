using Microsoft.AspNetCore.Mvc;

namespace MyCaRt.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login_Register()
        {
            return View();
        }
    }
}
