using Microsoft.AspNetCore.Mvc;
using MyCaRt.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MyCaRt.Helper;


namespace MyCaRt.Controllers
{
    public class LoginController : Controller
    {
        public readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public LoginController(IConfiguration config)
        {
            _config = config;
            //_httpClient = httpClientFactory.CreateClient();
           _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUri"]);

        }
        [HttpGet("Login_Register")]
        public IActionResult Login_Register()
        {
            return View();
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(LoginModel model)
        {

            var encryptedRegisterModel = new LoginModel
            {
                UserName= model.UserName,
                Email= model.Email,
                Password = EncryptionHelper.EncryptString(model.Password),
                CPassword = EncryptionHelper.EncryptString(model.CPassword)
               
            };
            var content = new StringContent(JsonConvert.SerializeObject(encryptedRegisterModel), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Login/Register", content);
            var result = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(result);
            if (data.message == "Registration successful")
            {
                TempData["SuccessMessage"]= data.message;
                return View("Login_Register");
            }
            ViewBag.Message = data.message;
            return View("Login_Register");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
           
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Login/Login", content);
            var result = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(result);

            if (data.message == "Login successful")
            {
                HttpContext.Session.SetString("User", model.Email);
                HttpContext.Session.SetString("SessionId", (string)data.sessionId);

                // Retrieving session values for logging
                var userEmail = HttpContext.Session.GetString("User");
                var sessionId = HttpContext.Session.GetString("SessionId");

                // Logging to console (this will log in your server's console, not the browser's console)
                Console.WriteLine($"User Email: {userEmail}");
                Console.WriteLine($"Session ID: {sessionId}");
                return RedirectToAction("Dashboard");
            }
            ViewBag.Message = data.message;
            return View("Login_Register");
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            var userEmail = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login_Register");
            }
            ViewBag.UserEmail = userEmail;
            return View();
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login_Register");
        }












    }
}
