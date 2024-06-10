using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyCaRt.Models;
using Newtonsoft.Json;
using System.Text;

namespace MyCaRt.Controllers
{
    public class ProductController : Controller
    {
        public readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ProductController(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUri"]);
        }

//get the productcategory to display in the downdown menu  to create product
        public async Task<IActionResult> CreateProduct()
        {
            List<ProductCategoryModel> categoryData = new List<ProductCategoryModel>();

            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/ProductCategory/GetProductCategory");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                categoryData = JsonConvert.DeserializeObject<List<ProductCategoryModel>>(data);
            }
            else
            {
                // Handle the error accordingly
                return NotFound();
            }

            ViewBag.categoryData = new SelectList(categoryData, "Category_Id", "Category_Name");

            return View();
        }



//create product with the productcategory id
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductModel product)
        {


            // Convert the employee model to JSON
            var json = JsonConvert.SerializeObject(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Make a POST request to the API
            HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Product/InsertProduct_Details", content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction("CreateProduct");
            }
            else
            {
                // Handle error response
                ModelState.AddModelError(string.Empty, "Server error. Please contact suryakumar.");
                return View(product); // Return the view with the form and model to show errors
            }

        }
 //list of full product
        public async Task<IActionResult> ListOfFullProduct()
        {



            List<ProductModel> products = new List<ProductModel>();

            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Product/GetAllProduct");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                products = JsonConvert.DeserializeObject<List<ProductModel>>(data);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact the administrator.");
            }


            List<Customer_NameModel> customerData = new List<Customer_NameModel>();

            response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Customer/GetAllCustomer_Name");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                customerData = JsonConvert.DeserializeObject<List<Customer_NameModel>>(data);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to load customer data.");
            }

            var customerNameItems = customerData.Select(c => new SelectListItem
            {
                Text = c.Customer_FName,
                Value = c.Customer_Id.ToString()
            }).ToList();

            ViewBag.categoryData = customerNameItems;



            List<ProductCategoryModel> productData = new List<ProductCategoryModel>();

            HttpResponseMessage response2 = await _httpClient.GetAsync(_httpClient.BaseAddress + "/ProductCategory/GetProductCategory");

            if (response2.IsSuccessStatusCode)
            {
                string data = await response2.Content.ReadAsStringAsync();
                productData = JsonConvert.DeserializeObject<List<ProductCategoryModel>>(data);
            }
            else
            {
                // Handle the error accordingly, e.g., log the error, set a default message, etc.
                ModelState.AddModelError(string.Empty, "Failed to load category data.");
            }

            ViewBag.productData = productData;


            return View(products);
        }
 //filter by products
        [HttpPost]
        public async Task<IActionResult> FilterProducts([FromBody] List<string> selectedCategories)
        {
            List<ProductModel> products = new List<ProductModel>();
            try
            {
                // Adjust the endpoint as per your API setup
                HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Product/GetAllProduct");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<List<ProductModel>>(data);

                    // Filter products based on selected categories
                    if (selectedCategories != null && selectedCategories.Any())
                    {
                        products = products.Where(p => selectedCategories.Contains(p.Product_Category)).ToList();
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Failed to load products" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, products });
        }



    }
}
