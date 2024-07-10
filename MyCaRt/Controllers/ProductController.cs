using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyCaRt.Models;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using System.Reflection;
using System.Text;
using static MyCaRt.Controllers.ProductCategoryController;

namespace MyCaRt.Controllers
{
    public class ProductController : Controller
    {
        public readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IConfiguration config, IWebHostEnvironment webHostEnvironment)
        {
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUri"]);
            _webHostEnvironment = webHostEnvironment;
        }

        private async Task<bool> SetCategoryDataAsync()
        {
            List<ProductCategoryModel> categoryData = new List<ProductCategoryModel>();
            HttpResponseMessage categoryResponse = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/ProductCategory/GetProductCategory");

            if (categoryResponse.IsSuccessStatusCode)
            {
                string data = await categoryResponse.Content.ReadAsStringAsync();
                categoryData = JsonConvert.DeserializeObject<List<ProductCategoryModel>>(data);
                ViewBag.CategoryData = new SelectList(categoryData, "Category_Id", "Category_Name");
                return true;
            }
            return false;
        }

        //get the productcategory to display in the downdown menu  to create product
        public async Task<IActionResult> CreateProduct()
        {

            if (!await SetCategoryDataAsync())
            {
                return NotFound();
            }

            return View(new List<ProductModel> { new ProductModel() });
        }

        public async Task<IActionResult> TestCreateProduct()
        {

            if (!await SetCategoryDataAsync())
            {
                return NotFound();
            }

            return View(new List<ProductModel> { new ProductModel() });
        }
        //store the image and get the storedpath
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile ImageName)
        {
            if (ImageName != null && ImageName.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var timestamp = DateTime.Now.ToString("-yyyy-MM-ddHHmmss");
                var fileName = Path.GetFileNameWithoutExtension(ImageName.FileName) + timestamp + Path.GetExtension(ImageName.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await ImageName.CopyToAsync(fileStream);

                return Ok(new { filePath = "/uploads/" + fileName, fileName });
            }

            return BadRequest("Invalid image file.");
        }

        //create product with the productcategory id
        [HttpPost]
        public async Task<IActionResult> CreateProduct(List<ProductModel> products)
        {
           
            if (ModelState.IsValid)
            {
            
                try
                {
                        // Convert the product model to JSON
                        var json = JsonConvert.SerializeObject(products);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        // Make a POST request to the API to insert product details
                        HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Product/InsertProduct_Details", content);

                        // Check if the request was successful
                        if (!response.IsSuccessStatusCode)
                        {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        TempData["ErrorMessage"] = errorContent;
                        return Json(new { success = false, errorContent });
                    }

                    return Ok(new { success = true, message = "Products added successfully." });

                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Server error please try later.";
                    return View(products);
                }
            }


            // If model state is not valid, re-render the form with validation errors
            var errors = ModelState
             .SelectMany(ms => ms.Value.Errors.Select(e => new { Field = ms.Key, Error = e.ErrorMessage }))
             .ToList();

            return Json(new { success = false, errors });

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
