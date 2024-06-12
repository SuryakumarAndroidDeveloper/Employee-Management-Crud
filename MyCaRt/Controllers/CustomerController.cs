using Microsoft.AspNetCore.Mvc;
using MyCaRt.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static MyCaRt.Controllers.ProductController;

namespace MyCaRt.Controllers
{
    public class CustomerController : Controller
    {
        public readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public CustomerController(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUri"]);
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


        //create Customer 

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
                HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Customer/InsertCustomerDetails", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Customer created successfully!";
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

        //display the customer
       
        public async Task<IActionResult> ListOfCustomers()
        {

            List<CustomerInterestedCategory> customers = new List<CustomerInterestedCategory>();

            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Customer/GetAllCustomer");
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


        //edit the customerdetails the customerdetails fetch to the field based on customerid
        public IActionResult Edit()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            CustomerModel customer = null;

            HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Customer/GetCustomerById?id={id}");
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
        //update the customer details based on customerid

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CustomerModel customer)
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
            HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Customer/UpdateCustomerDetails?id={id}", content);

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

        //deactivate the customerdetails based on customerid

        [HttpPost]
        public async Task<IActionResult> Deactivate(int id)

        {
            HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Customer/DeleteCustomerById?id={id}", null);

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

        //myorders based on customerid
        public IActionResult MyOrders()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders(int Customer_Id)
        {

            List<MyOrderModel> orderData = new List<MyOrderModel>();

            HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Order/GetOrderByCustomer?customerId={Customer_Id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                orderData = JsonConvert.DeserializeObject<List<MyOrderModel>>(data);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ViewData["Message"] = "No orders found.";
                return View(orderData);
            }
            else
            {
                return BadRequest("Failed to load order data.");
            }

            return View(orderData);
        }
        //get mywishlist based on customerid

        public IActionResult MyWishList()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyWishList(int Customer_Id)
        {


            List<MyWishlistModel> wishListData = new List<MyWishlistModel>();

            HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/WishList/GetWishListByCustomer?customerId={Customer_Id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                wishListData = JsonConvert.DeserializeObject<List<MyWishlistModel>>(data);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ViewData["Message"] = "No WishList found.";
                return View(wishListData);
            }
            else
            {
                return BadRequest("Failed to load wishlist data.");
            }

            return View(wishListData);
        }
        //validate customer firstname
        [HttpPost]
        public async Task<IActionResult> IsCustomer_FNameAvailable([FromBody] CustomerNamesRequest request)
        {
            if (string.IsNullOrEmpty(request.Customer_FName))
            {
                return Json(new { Exists = false });
            }

            var json = JsonConvert.SerializeObject(new { Customer_FName = request.Customer_FName });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Customer/IsCustomer_FNameAvailable", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var existsResponse = JsonConvert.DeserializeObject<ExistsResponse>(result);
                return Json(new { Exists = existsResponse.Exists });
            }
            else
            {
                return Json(new { Exists = false });
            }
        }

        public class CustomerNamesRequest
        {
            public string Customer_FName { get; set; }
        }

        public class ExistsResponse
        {
            public bool Exists { get; set; }
        }

        //validate email
        [HttpPost]
        public async Task<IActionResult> IsEmailAvailable([FromBody] EmailRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return Json(new { Exists = false });
            }

            var json = JsonConvert.SerializeObject(new { email = request.Email });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Customer/IsEmailAvailable", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var existsResponse = JsonConvert.DeserializeObject<ExistsResponse>(result);
                return Json(new { Exists = existsResponse.Exists });
            }
            else
            {
                return Json(new { Exists = false });
            }
        }
        public class EmailRequest
        {
            public string Email{ get; set; }
        }

       
        //validate mobile
        [HttpPost]
        public async Task<IActionResult> IsMobileAvailable([FromBody] MobileRequest request)
        {
            if (string.IsNullOrEmpty(request.Mobile))
            {
                return Json(new { Exists = false });
            }

            var json = JsonConvert.SerializeObject(new { mobile = request.Mobile });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Customer/IsMobileAvailable", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var existsResponse = JsonConvert.DeserializeObject<ExistsResponse>(result);
                return Json(new { Exists = existsResponse.Exists });
            }
            else
            {
                return Json(new { Exists = false });
            }
        }
        public class MobileRequest
        {
            public string Mobile { get; set; }
        }

   

    }
}
