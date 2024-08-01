using Microsoft.AspNetCore.Mvc;
using MyCaRt.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MyCaRt.Helper;
using System.Data;
using System.Reflection;
using System.Net.Mail;
using System.Net;
using static System.Net.WebRequestMethods;
using System.Linq.Expressions;


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
        public async Task<IActionResult> Register(RegisterUserModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ShowSignup = true;
                return View("Login_Register", model);
            }

            var encryptedRegisterModel = new RegisterUserModel
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = EncryptionHelper.EncryptString(model.Password),
                CPassword = EncryptionHelper.EncryptString(model.CPassword)

            };
            var content = new StringContent(JsonConvert.SerializeObject(encryptedRegisterModel), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Login/Register", content);
            var result = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(result);
            if (data.message == "Registration successful")
            {
                TempData["SuccessMessage"] = data.message;
                return View("Login_Register");
            }
            ViewBag.Message = data.message;
            ViewBag.ShowSignup = true;
            return View("Login_Register", model);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login_Register", model);
            }

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Login/Login", content);
            var result = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(result);

            if (data.message == "Login successful")
            {
                HttpContext.Session.SetString("User", model.Email);
                HttpContext.Session.SetString("SessionId", (string)data.sessionId);
                HttpContext.Session.SetInt32("Role", (int)data.role);
                HttpContext.Session.SetInt32("Userid", (int)data.userid);

                // Retrieving session values for logging
                var userEmail = HttpContext.Session.GetString("User");
                var sessionId = HttpContext.Session.GetString("SessionId");
                var roleid = HttpContext.Session.GetInt32("Role");
                var userid = HttpContext.Session.GetInt32("Userid");

                // Logging to console (this will log in your server's console, not the browser's console)
                Console.WriteLine($"User Email: {userEmail}");
                Console.WriteLine($"Session ID: {sessionId}");
                Console.WriteLine($"Role ID: {roleid}");
                Console.WriteLine($"User ID: {userid}");

                if (data.role == 9001) // User role 
                {
                    return RedirectToAction("UserDashboard", "User");

                }
                else// Admin role
                {
                    return RedirectToAction("AdminDashboard", "Admin");
                }

            }
            TempData["InvalidMessage"] = data.message;
            return View("Login_Register");
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login_Register");
        }



        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(RegisterUserModel model)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Login/ForgetPassword", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(result);
                    //  TempData["Password"] = data.password;

                    if (data.message == "Password Found")
                    {
                        // Generate OTP
                        string otp = GenerateOTP();

                        // Send OTP to Email
                        bool isOtpSent = SendOTPEmail(model.Email, otp);

                        if (isOtpSent)
                        {

                            var url = $"https://localhost:7156/Login/ResetPassword?{otp}";
                            // Store OTP in session
                            //  HttpContext.Session.SetString("OTP", otp);
                            //  HttpContext.Session.SetString("url", url);
                            var userEmail = EncryptionHelper.EncryptString(model.Email);
                            // HttpContext.Session.SetString("UserEmail",userEmail);
                            //  HttpContext.Session.SetString("Password", (string)data.password);
                            // TempData["Message"] = "ResetPassword link has been sent to your email.";
                            // Create request payload
                            var requestModel = new OtpRequestModel
                            {
                                Email = userEmail,
                                Url = url
                            };

                            var json = JsonConvert.SerializeObject(requestModel);
                            var content1 = new StringContent(json, Encoding.UTF8, "application/json");

                            HttpResponseMessage response1 = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Login/SendResetPasswordLink", content1);

                            if (response1.IsSuccessStatusCode)
                            {
                                TempData["Message"] = "ResetPassword link has been sent to your email.";
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Failed to send ResetPassword link.";
                            }
                        }

                        else
                        {
                            TempData["Message"] = "Failed to send OTP to your email.";
                        }
                        //ViewBag.Message = "Your password is: " + data.password;
                    }
                    else
                    {
                        TempData["Message"] = data.message;
                    }
                }
                else
                {
                    TempData["Message"] = "Failed to call API.";
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            return View("Login_Register");
        }

   /*     public IActionResult EnterOTP()
        {
            return View();
        }

        [HttpPost("VerifyOTP")]
        public IActionResult VerifyOTP(string otp)
        {
            string sessionOTP = HttpContext.Session.GetString("OTP");

            if (string.IsNullOrEmpty(sessionOTP))
            {
                ViewBag.Message = "OTP session expired or invalid. Please try again.";
                return View("Login_Register");
            }

            if (otp == sessionOTP)
            {
                // Clear OTP session after successful verification
                HttpContext.Session.Remove("OTP");
                // Retrieve the password from session
                string password = HttpContext.Session.GetString("Password");

                if (!string.IsNullOrEmpty(password))
                {
                    ViewBag.MessageSuccess = $"OTP verified successfully. Your password is: {password}";
                    HttpContext.Session.Remove("Password");
                }
                else
                {
                    ViewBag.Message = "Password retrieval failed. Please try again.";
                }
            }
            else
            {
                ViewBag.Message = "Invalid OTP. Please try again.";
                return View("EnterOTP");
            }

            return View("Login_Register");
        }*/
        //resetpassword view if the url is present in the session

        public async Task <IActionResult> ResetPassword()
        {
            //string sessionURL = HttpContext.Session.GetString("url");
            string fullUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
            string queryString = Request.QueryString.Value;
            if (string.IsNullOrEmpty(queryString))
            {
                TempData["ResetMessage"] = "Please Verify the email";
                return View("Login_Register");
            }
            // Call the Web API to get the OTP request data
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + $"/Login/GetOtpRequest?otpUrl={fullUrl}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var otpRequest = JsonConvert.DeserializeObject<OtpRequestModel>(jsonResponse);

                if (otpRequest.Url == fullUrl)
                {
                    TimeSpan elapsed = DateTime.UtcNow - otpRequest.CreatedAt;
                    if (elapsed.TotalSeconds <= 60)
                    {
                        var remainingTime = 60 - (int)elapsed.TotalSeconds;
                        ViewBag.RemainingTime = remainingTime;
                        HttpContext.Session.SetString("UserEmail", otpRequest.Email);
                        return View();
                    }
                    else
                    {
                        TempData["ResetMessage"] = "The reset URL has expired. Please request a new one.";
                        return View("Login_Register");
                    }
                }
                else
                {
                    TempData["ResetMessage"] = "Wrong URL entered. Please check the email.";
                    return View("Login_Register");
                }
            }
            else
            {
                TempData["ResetMessage"] = "Invalid or expired reset URL. Please request a new one.";
                return View("Login_Register");
            }
        
        /*         else if (sessionURL== fullUrl)
                 {
                     HttpContext.Session.Remove("url");
                     return View();
                 }
                 TempData["ResetMessage"] = "Wrong Url have been entered please check the email";
                 return View("Login_Register");*/
    }

        //resetpassword will update to the userpassword field
        // string userEmail = HttpContext.Session.GetString("Password");

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        
        {
            
            string email = HttpContext.Session.GetString("UserEmail");
            var userEmail = EncryptionHelper.DecryptString(email);
            var encryptedResetPasswordModel = new ResetPasswordModel
            {
                Email = userEmail,
                Password = EncryptionHelper.EncryptString(model.Password),
                CPassword = EncryptionHelper.EncryptString(model.CPassword)

            };
            var content = new StringContent(JsonConvert.SerializeObject(encryptedResetPasswordModel), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Login/ResetPassword", content);
            var result = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(result);
            if (data.message == "Password Updated Successfully")
            {
                TempData["SuccessMessage"] = data.message;
                HttpContext.Session.Remove("UserEmail");
                return View("Login_Register");
            }
            TempData["Message"]  = data.message;
            return View("Login_Register", model);
        }

        // Method to generate a random OTP
        private string GenerateOTP()
        {
            Random random = new Random();
            int otpValue = random.Next(100000, 999999); // Generate a 6-digit OTP (guid)
            return otpValue.ToString();
        }

        // Method to send OTP to email using SMTP
        private bool SendOTPEmail(string email, string otp)
        {
            try
            {
                var url = $"https://localhost:7156/Login/ResetPassword?{otp}";
                string fromEmail = "mycart866@gmail.com"; 
                string fromEmailPassword = "cgoz bisb oubb xdqb"; 
                string subject = "Password Reset OTP";
                string body = $"Your ResetPassword Link is: {url}";


                //.netcore 8 simplify the code for object creation
                using MailMessage mailMessage = new();
                mailMessage.From = new MailAddress(fromEmail);
                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = false;

                using SmtpClient smtpClient = new("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(fromEmail, fromEmailPassword);
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                return false;
            }
        }














    }
}
