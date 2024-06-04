using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyCaRt.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyCaRt.Controllers
{
    public class ProductCategoryController : Controller
    {
        public readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ProductCategoryController(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUri"]);

        }


        public IActionResult Index()
        {
            return View();
        }


        public IActionResult CreateProductCategory()
        {
            return View();
        }

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
            }

            ViewBag.categoryData = new SelectList(categoryData, "Category_Id", "Category_Name");

            return View();
        }

        public async Task<IActionResult> Customer()
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
                // Handle the error accordingly, e.g., log the error, set a default message, etc.
                ModelState.AddModelError(string.Empty, "Failed to load category data.");
            }

            ViewBag.categoryData = categoryData;

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> CreateProductCategory(ProductCategoryModel productCategory)
        {

            // Convert the employee model to JSON
            var json = JsonConvert.SerializeObject(productCategory);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Make a POST request to the API
            HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/ProductCategory/InsertProductCategory", content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                //string data = await response.Content.ReadAsStringAsync();
                // EmployeModel createdEmployee = JsonConvert.DeserializeObject<EmployeModel>(data);

                // Optionally, return a view with the created employee or a success message
                //  return View("Index"); // Assuming you have a view named "EmployeeDetails"

                return RedirectToAction("CreateProductCategory");
            }
            else
            {
                // Handle error response
                ModelState.AddModelError(string.Empty, "Server error. Please contact suryakumar.");
                return View(productCategory); // Return the view with the form and model to show errors
            }


        }



        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductModel product)
        {


            // Convert the employee model to JSON
            var json = JsonConvert.SerializeObject(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Make a POST request to the API
            HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/ProductCategory/InsertProduct_Details", content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("CreateProduct");
            }
            else
            {
                // Handle error response
                ModelState.AddModelError(string.Empty, "Server error. Please contact suryakumar.");
                return View(product); // Return the view with the form and model to show errors
            }


        }



        [HttpPost]
        public async Task<IActionResult> Customer(CustomerModel customer)
        {
            List<ProductCategoryModel> categoryData = new List<ProductCategoryModel>();

            HttpResponseMessage response1 = await _httpClient.GetAsync(_httpClient.BaseAddress + "/ProductCategory/GetProductCategory");

            if (response1.IsSuccessStatusCode)
            {
                string data = await response1.Content.ReadAsStringAsync();
                categoryData = JsonConvert.DeserializeObject<List<ProductCategoryModel>>(data);
            }
            else
            {
                // Handle the error accordingly, e.g., log the error, set a default message, etc.
                ModelState.AddModelError(string.Empty, "Failed to load category data.");
            }

            ViewBag.categoryData = categoryData;
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            var json = JsonConvert.SerializeObject(customer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/ProductCategory/InsertCustomerDetails", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Customer");
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response Status Code: {response.StatusCode}");
                    Console.WriteLine($"Response Content: {responseContent}");
                    ModelState.AddModelError(string.Empty, $"Server error: {responseContent}");
                    // Re-fetch categories in case of error
                    return View(customer);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Server error. Please contact the administrator.");
                return View(customer);
            }

        }


        public async Task<IActionResult> ListOfCustomers()
        {

            List<CustomerInterestedCategory> customers = new List<CustomerInterestedCategory>();

            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/ProductCategory/GetAllCustomer");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                customers = JsonConvert.DeserializeObject<List<CustomerInterestedCategory>>(data);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact the administrator.");
            }

            return View(customers);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            CustomerModel customer = null;

            HttpResponseMessage response = await _httpClient.GetAsync($"{ _httpClient.BaseAddress}/ProductCategory/GetCustomerById?id={id}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(data);
                customer = JsonConvert.DeserializeObject<CustomerModel>(data);
                List<ProductCategoryModel> categoryData = new List<ProductCategoryModel>();

                HttpResponseMessage response1 = await _httpClient.GetAsync(_httpClient.BaseAddress + "/ProductCategory/GetProductCategory");

                if (response1.IsSuccessStatusCode)
                {
                    string data1 = await response1.Content.ReadAsStringAsync();
                    categoryData = JsonConvert.DeserializeObject<List<ProductCategoryModel>>(data1);
                }
                else
                {
                    // Handle the error accordingly, e.g., log the error, set a default message, etc.
                    ModelState.AddModelError(string.Empty, "Failed to load category data.");
                }

                ViewBag.categoryData = categoryData;

               
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact the administrator.");
            }
            return View(customer);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,CustomerModel customer)
        {
            // Serialize the customer object to JSON
            var json = JsonConvert.SerializeObject(customer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Get the list of product categories
            List<ProductCategoryModel> categoryData = new List<ProductCategoryModel>();
            HttpResponseMessage response1 = await _httpClient.GetAsync(_httpClient.BaseAddress + "/ProductCategory/GetProductCategory");

            if (response1.IsSuccessStatusCode)
            {
                string data = await response1.Content.ReadAsStringAsync();
                categoryData = JsonConvert.DeserializeObject<List<ProductCategoryModel>>(data);
            }
            else
            {
                // Handle the error accordingly
                ModelState.AddModelError(string.Empty, "Failed to load category data.");
            }

            ViewBag.categoryData = categoryData;

            // Make the POST request to update customer details
            HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/ProductCategory/UpdateCustomerDetails?id={id}", content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Redirect to the list of customers
                return RedirectToAction("ListOfCustomers");
            }
            else
            {
                // If there's an error, add model error and return the view
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Status Code: {response.StatusCode}");
                Console.WriteLine($"Response Content: {responseContent}");
                ModelState.AddModelError(string.Empty, $"Server error: {responseContent}");
                return View(customer);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Deactivate(int id)

        {
            HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/ProductCategory/DeleteCustomerById?id={id}", null);

            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("ListOfCustomers");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact support.");
                return RedirectToAction("ListOfCustomers");
            }
        }

        public async Task<IActionResult> ListOfFullProduct()
        {



            List<ProductModel> products = new List<ProductModel>();

            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/ProductCategory/GetAllProduct");
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

            response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/ProductCategory/GetAllCustomer_Name");

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


        [HttpPost]
        public async Task<IActionResult> AddToCart(CartItemModel cartItem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<ProductCategoryModel> categoryData = new List<ProductCategoryModel>();

                    HttpResponseMessage response2 = await _httpClient.GetAsync(_httpClient.BaseAddress + "/ProductCategory/GetProductCategory");

                    if (response2.IsSuccessStatusCode)
                    {
                        string data = await response2.Content.ReadAsStringAsync();
                        categoryData = JsonConvert.DeserializeObject<List<ProductCategoryModel>>(data);
                    }
                    else
                    {
                        // Handle the error accordingly, e.g., log the error, set a default message, etc.
                        ModelState.AddModelError(string.Empty, "Failed to load category data.");
                    }

                    ViewBag.categoryData = categoryData;
                    // Fetch customer data
                    List<Customer_NameModel> customerData = new List<Customer_NameModel>();
                    HttpResponseMessage response1 = await _httpClient.GetAsync(_httpClient.BaseAddress + "/ProductCategory/GetAllCustomer_Name");

                    if (response1.IsSuccessStatusCode)
                    {
                        string data = await response1.Content.ReadAsStringAsync();
                        customerData = JsonConvert.DeserializeObject<List<Customer_NameModel>>(data);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Failed to load customer data.");
                        // Handle the error appropriately
                    }

                    // Prepare customer data for view
                    var customerNameItems = customerData.Select(c => new SelectListItem
                    {
                        Text = c.Customer_FName,
                        Value = c.Customer_Id.ToString()
                    }).ToList();

                    ViewBag.categoryData = customerNameItems;

                    // Serialize the cart item
                    var jsonCartItem = JsonConvert.SerializeObject(cartItem);

                    // Create the request content
                    var content = new StringContent(jsonCartItem, Encoding.UTF8, "application/json");

                    // Post the cart item to the API
                    HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/ProductCategory/AddToCart", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true });
                        //return RedirectToAction("ListOfFullProduct");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Server error. Please contact support.");
                        // Handle the error appropriately
                        return Json(new { success = false, message = "Invalid model state" });
                       // return RedirectToAction("ListOfCustomers");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                    // Handle the error appropriately
                    return Json(new { success = false, message = ex.Message });
                    //return RedirectToAction("ListOfCustomers");
                }
            }

            return View("Index");
        }


        [HttpPost]
        public async Task<IActionResult> FilterProducts([FromBody] List<string> selectedCategories)
        {
            List<ProductModel> products = new List<ProductModel>();
            try
            {
                // Adjust the endpoint as per your API setup
                HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/ProductCategory/GetAllProduct");
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
