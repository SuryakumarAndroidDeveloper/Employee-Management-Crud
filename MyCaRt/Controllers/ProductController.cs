using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyCaRt.Models;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System.Net.Http;
using System.Reflection;
using System.Text;
using static MyCaRt.Controllers.ProductCategoryController;
using static MyCaRt.Enum.@enum;

namespace MyCaRt.Controllers
{


    [CustomAuthorize(UserRoles.Admin)]
    public class ProductController : Controller
    {
        public readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductController(IConfiguration config, IWebHostEnvironment webHostEnvironment, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUri"]);
            _webHostEnvironment = webHostEnvironment;
            _httpClientFactory = httpClientFactory;
        }

        protected int? UserRole;
        protected int? UserId;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            UserRole = HttpContext.Session.GetInt32("Role");
            ViewBag.UserRole = UserRole;
            UserId = HttpContext.Session.GetInt32("Userid");
            ViewBag.UserId = UserId;
            base.OnActionExecuting(context);
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
          /*  int? role = UserRole;

            if (role == 9002)
            {*/

                if (!await SetCategoryDataAsync())
                {
                    return NotFound();
                }

                return View(new List<ProductModel> { new ProductModel() });
           /* }
            TempData["AlertMessage"] = "You need admin privileges to access this page.";
            return RedirectToAction("Login_Register", "Login");*/
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
        [CustomAuthorize(UserRoles.Admin, UserRoles.User)]
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
        [CustomAuthorize(UserRoles.Admin, UserRoles.User)]
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










        public IActionResult UploadExcel()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return View("Error", new { message = "No file uploaded." });

            string jsonData;
            var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadDir);
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                // Set the license context
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;
                    int imageColumnIndex = 7;
                    var dataList = new List<ProductModel>();
                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var imageUrls = worksheet.Cells[row, imageColumnIndex].Text.Split(';');// Assuming multiple URLs are separated by semicolons
                        var filePaths = new List<string>();
                        var fileNames = new List<string>();
                        foreach (var imageUrl in imageUrls)
                        {
                            if (!string.IsNullOrWhiteSpace(imageUrl))
                            {
                                var filePath = await DownloadAndSaveImageAsync(imageUrl.Trim(), uploadDir);
                                filePaths.Add(filePath);

                            }
                        }


                         dataList.Add(new ProductModel
                        {
                            Product_Category = worksheet.Cells[row, 1].Value.ToString(),
                            Product_Code = worksheet.Cells[row, 2].Value.ToString(),
                            Product_Name = worksheet.Cells[row, 3].Value.ToString(),
                            Product_Price = Convert.ToDecimal(worksheet.Cells[row, 4].Value),
                            Product_Description = worksheet.Cells[row, 5].Value.ToString(),
                            Available_Quantity = Convert.ToInt32(worksheet.Cells[row, 6].Value),
                            ImageName = worksheet.Cells[row, 7].Value.ToString(),
                            FilePath = string.Join(";", filePaths)
                        });
                    }
                jsonData = JsonConvert.SerializeObject(dataList);
                }
            }

           // var client = _httpClientFactory.CreateClient();
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Product/InsertProductJsonData", content);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Data inserted successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Error inserting data." });
            }
        }

        private async Task<string> DownloadAndSaveImageAsync(string imageUrl, string uploadDir)
        {
            if (Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                // Handle URL case
                var fileName = Path.GetFileName(imageUrl);
                fileName = SanitizeFileName(fileName);
                var filePath = Path.Combine(uploadDir, fileName);

                // Ensure unique filename
                if (System.IO.File.Exists(filePath))
                {
                    var timestamp = DateTime.Now.ToString("-yyyy-MM-ddHHmmss");
                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{timestamp}_{DateTime.Now.Ticks}{Path.GetExtension(fileName)}";
                    // fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.Now.Ticks}{Path.GetExtension(fileName)}";
                    filePath = Path.Combine(uploadDir, fileName);
                }

                using (var client = _httpClientFactory.CreateClient())
                {
                    var response = await client.GetAsync(imageUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await response.Content.CopyToAsync(fileStream);
                            }
                            return Path.Combine("/uploads", fileName).Replace("\\", "/");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error saving image from URL: {ex.Message}");
                            return null;
                        }
                    }
                }
            }
            else
            {
                // Handle local file path case
                try
                {
                    var filePath = Path.GetFullPath(imageUrl);
                    Console.WriteLine($"Checking local file path: {filePath}");

                    // Validate the source file path
                    if (!System.IO.File.Exists(filePath))
                    {
                        Console.WriteLine($"Local file not found: {filePath}");
                        return null;
                    }

                    var fileName = Path.GetFileName(filePath);
                    fileName = SanitizeFileName(fileName);
                    var destinationFilePath = Path.Combine(uploadDir, fileName);

                    // Ensure unique filename
                    if (System.IO.File.Exists(destinationFilePath))
                    {
                        var timestamp = DateTime.Now.ToString("-yyyy-MM-ddHHmmss");
                        fileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{timestamp}_{DateTime.Now.Ticks}{Path.GetExtension(fileName)}";
                      
                        destinationFilePath = Path.Combine(uploadDir, fileName);
                    }

                    // Copy the local file to the upload directory
                    System.IO.File.Copy(filePath, destinationFilePath);
                    return Path.Combine("/uploads", fileName).Replace("\\", "/");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Unauthorized access: {ex.Message}");
                    return null;
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"IO error: {ex.Message}");
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error copying local image file: {ex.Message}");
                    return null;
                }
            }
            return null;
        }



        private string SanitizeFileName(string fileName)
        {
            // Remove invalid characters from file name
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitizedFileName = new string(fileName
                .Select(c => invalidChars.Contains(c) ? '_' : c)
                .ToArray());

            // You might want to truncate the filename if it's too long
            return sanitizedFileName.Length > 255 ? sanitizedFileName.Substring(0, 255) : sanitizedFileName;
        }





    }
}
