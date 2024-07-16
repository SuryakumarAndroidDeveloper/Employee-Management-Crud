using Microsoft.AspNetCore.Mvc;
using static MyCaRt.Enum.@enum;

namespace MyCaRt.Controllers
{
    [CustomAuthorize(UserRoles.User)]
    public class UserController : Controller
    {
        public readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public UserController(IConfiguration config)
        {
            _config = config;
            //_httpClient = httpClientFactory.CreateClient();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUri"]);

        }
        [HttpGet("UserDashboard")]
        public IActionResult UserDashboard()
        {
            var userEmail = HttpContext.Session.GetString("User");
            var userRole = HttpContext.Session.GetInt32("Role");
            var userId = HttpContext.Session.GetInt32("Userid");
            if (string.IsNullOrEmpty(userEmail) && userRole != 9001)
            {
                return RedirectToAction("Login_Register","Login");
            }
            ViewBag.UserEmail = userEmail;
            ViewBag.UserRole = userRole;
            ViewBag.UserId = userId;
            return View();
        }
     
    }
}
