using CRUDApiApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CRUDApiApplication.Controllers
{
    public class EmployeController : Controller
    {

        //Uri uri = new Uri("https://localhost:7065/api");

        public readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public EmployeController(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUri"]);
        }


        [HttpGet]
        public IActionResult Index()
        {
            // Initialize an empty list to hold employee data
            List<EmployeModel> employe = new List<EmployeModel>();
            // Make a synchronous GET request to the API endpoin
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Employe/GetAllEmployees").Result;
            // Check if the response indicates success (HTTP status code 200)
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                String data = response.Content.ReadAsStringAsync().Result;
                // Deserialize the JSON string into a list of EmployeModel objects
                employe = JsonConvert.DeserializeObject<List<EmployeModel>>(data);
            }
            return View(employe);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(EmployeModel employee)
        {
            // Convert the employee model to JSON
            var json = JsonConvert.SerializeObject(employee);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Make a POST request to the API
            HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Employe/InsertEmployee", content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                //string data = await response.Content.ReadAsStringAsync();
                // EmployeModel createdEmployee = JsonConvert.DeserializeObject<EmployeModel>(data);

                // Optionally, return a view with the created employee or a success message
                //  return View("Index"); // Assuming you have a view named "EmployeeDetails"

                return RedirectToAction("Index");
            }
            else
            {
                // Handle error response
                ModelState.AddModelError(string.Empty, "Server error. Please contact suryakumar.");
                return View(employee); // Return the view with the form and model to show errors
            }


        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Employe/GetEmployeeById?id={id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                EmployeModel employe = JsonConvert.DeserializeObject<EmployeModel>(data);
                return View(employe);
            }
            else
            {
                // Handle the case when the employee is not found or there is an error
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeModel employee)
        {
            var json = JsonConvert.SerializeObject(employee);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Construct the full URI with query parameters
           // string requestUri = $"{uri}/Employe/UpdateEmployeeById?id={employee.Id}";

            // Make the POST request to the API
            HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Employe/UpdateEmployeeById?id={employee.Id}", content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact support.");
                return View(employee);
            }
        }



        [HttpPost]
        public async Task<IActionResult> Deactivate(int id)
        
        {
            HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Employe/DeleteEmployeeById?id={id}", null);

            if (response.IsSuccessStatusCode)
            {
               
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact support.");
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {

            HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Employe/GetEmployeeById?id={id}");

            if (response.IsSuccessStatusCode)
            {    
                string data = await response.Content.ReadAsStringAsync();
                EmployeModel employe = JsonConvert.DeserializeObject<EmployeModel>(data);
                return View(employe);
            }
            else
            {
                // Handle the case when the employee is not found or there is an error
                return NotFound();
            }
        }


    








    }
}
