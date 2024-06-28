using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MyCaRt.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace MyCaRt.Controllers
{
    public class ProductCategoryController : Controller
    {
        public readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ProductCategoryController(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            var baseAddress = _config["ApiSettings:BaseUri"];
            
            if (string.IsNullOrEmpty(baseAddress))
            {
                throw new ArgumentNullException("BaseUri configuration is missing.");
            }

            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(baseAddress);
            }

        }


        public IActionResult Index()
        {
            return View();
        }


        public IActionResult CreateProductCategory()
        {
            
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateProductCategory(ProductCategoryModel productCategory)
        {
            if (!ModelState.IsValid)
            {
                return View(productCategory); // Return the view with the form and validation errors
            }
            if (productCategory == null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            // Convert the employee model to JSON
            var json = JsonConvert.SerializeObject(productCategory);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Make a POST request to the API
            HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/ProductCategory/InsertProductCategory", content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "ProductCategory created successfully!";

                // return RedirectToAction("CreateProductCategory");
                return RedirectToAction(nameof(CreateProductCategory));
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = errorContent;
                return View(productCategory); // Return the view with the form and model to show errors
            }


        }
    }
}
