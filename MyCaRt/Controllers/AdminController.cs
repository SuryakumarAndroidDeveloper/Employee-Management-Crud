using Microsoft.AspNetCore.Mvc;
using static MyCaRt.Enum.@enum;

namespace MyCaRt.Controllers
{
    [CustomAuthorize(UserRoles.Admin)]
    public class AdminController : Controller
    {
        public readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public AdminController(IConfiguration config)
        {
            _config = config;
            //_httpClient = httpClientFactory.CreateClient();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUri"]);

        }
        [HttpGet("AdminDashboard")]
        public IActionResult AdminDashboard()
        {
            HttpContext.Session.GetString("User");
            HttpContext.Session.GetInt32("Role");
            HttpContext.Session.GetString("SessionId");
            var userEmail = HttpContext.Session.GetString("User");
            var userRole = HttpContext.Session.GetInt32("Role");
            if (string.IsNullOrEmpty(userEmail) && userRole != 9001)
            {
                return RedirectToAction("Login_Register", "Login");
            }
            ViewBag.UserEmail = userEmail;
            ViewBag.UserRole = userRole;
            return View();
        }






    }
}
