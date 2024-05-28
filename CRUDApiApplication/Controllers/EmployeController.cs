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

        Uri uri = new Uri("https://localhost:7065/api");

        public readonly HttpClient _httpClient;

        public EmployeController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = uri;
        }


        [HttpGet]
        public IActionResult Index()
        {

            List<EmployeModel> employe = new List<EmployeModel>();
            HttpResponseMessage response = _httpClient.GetAsync(uri + "/Employe/GetAllEmployees").Result;

            if (response.IsSuccessStatusCode)
            {
                String data = response.Content.ReadAsStringAsync().Result;
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
            HttpResponseMessage response = await _httpClient.PostAsync(uri + "/Employe/InsertEmployee", content);

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
            HttpResponseMessage response = await _httpClient.GetAsync($"{uri}/Employe/GetEmployeeById?id={id}");

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
            // Serialize the employee object to JSON
            var json = JsonConvert.SerializeObject(employee);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Make a POST request to the API to update the employee
            HttpResponseMessage response = await _httpClient.PostAsync(uri+"/Employe/UpdateEmployeeById?id={employee.Id}", content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Optionally, return to the index view or another view after a successful update
                return RedirectToAction("Index");
            }
            else
            {
                // Handle error response
                ModelState.AddModelError(string.Empty, "Server error. Please contact support.");
                // Return the view with the form and model to show errors
                // Ensure that you're passing the ModelState along with the employee model
                return View(employee);
            }
        }














    }
}
