using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyCaRt.Models;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Text;
using static MyCaRt.Controllers.ProductController;
using static MyCaRt.Enum.@enum;

namespace MyCaRt.Controllers
{
    [CustomAuthorize(UserRoles.Admin)]
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
            
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = errorContent;
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
        [CustomAuthorize(UserRoles.Admin, UserRoles.User)]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpGet]
        [CustomAuthorize(UserRoles.Admin, UserRoles.User)]
        public async Task<IActionResult> ViewProfile(int id)
        {
            int? role = UserRole;
            int? userid = UserId;

            if (role == (int)UserRoles.Admin || (role == (int)UserRoles.User && userid == id))
            {
                CustomerModel customer = null;

            HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Customer/GetCustomerById?id={id}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(data);
                customer = JsonConvert.DeserializeObject<CustomerModel>(data);

            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact the administrator.");
                return BadRequest("Cannot View Profile.");
            }
           
            return View(customer);
            }
            TempData["AlertMessage"] = "You need login to access this page.";
            return RedirectToAction("Login_Register", "Login");
        }

        [HttpGet]
        [CustomAuthorize(UserRoles.Admin, UserRoles.User)]
        public async Task<IActionResult> Edit(int id)
        {
            int? role = UserRole;
            int? userid = UserId;

            if (role == (int)UserRoles.Admin || (role == (int)UserRoles.User && userid == id))
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
            TempData["AlertMessage"] = "You need login to access this page.";
            return RedirectToAction("Login_Register", "Login");
        }
        //update the customer details based on customerid

        [HttpPost]
        [CustomAuthorize(UserRoles.Admin, UserRoles.User)]
        public async Task<IActionResult> Edit(int id, CustomerModel customer)
        {
            int? role = UserRole;
            int? userid = UserId;

            if (role == (int)UserRoles.Admin || (role == (int)UserRoles.User && userid == id))
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
                    if (role == (int)UserRoles.Admin)
                    {
                        return RedirectToAction("ListOfCustomers");
                    }
                    TempData["SavedChanges"] = "Profile Updated Successfully";
                return RedirectToAction("ViewProfile", new { id = userid });
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
            TempData["AlertMessage"] = "You need login to access this page.";
            return RedirectToAction("Login_Register", "Login");
        }

        //deactivate the customerdetails based on customerid

        [HttpPost]
        [CustomAuthorize(UserRoles.Admin, UserRoles.User)]
        public async Task<IActionResult> Deactivate(int id)

        {
            int? role = UserRole;
            int? userid = UserId;

            if (role == (int)UserRoles.Admin || (role == (int)UserRoles.User && userid == id))
            {
                HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Customer/DeleteCustomerById?id={id}", null);

            if (response.IsSuccessStatusCode)
            {
                    if (role == (int)UserRoles.Admin)
                    {
                        return RedirectToAction("ListOfCustomers");
                    }
                    TempData["AlertMessage"] = "Account Deleted!";
                    return RedirectToAction("Login_Register", "Login");
                
               
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact support.");
                return RedirectToAction("ListOfCustomers");
            }
            }
            TempData["AlertMessage"] = "You need login to access this page.";
            return RedirectToAction("Login_Register", "Login");
        }

        //myorders based on customerid
        [CustomAuthorize(UserRoles.Admin, UserRoles.User)]
        public IActionResult MyOrders()
        {
                return View();

    }


        [HttpGet]
        [CustomAuthorize(UserRoles.Admin, UserRoles.User)]
        public async Task<IActionResult> MyOrders(int Customer_Id)
        {
            int? role = UserRole;
            int? userid = UserId;
            if (role == (int)UserRoles.Admin || (role == (int)UserRoles.User && userid == Customer_Id))
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
            /*TempData["AlertMessage"] = "You dont have access to this page.";*/
            return RedirectToAction("MyOrders", new { Customer_Id = userid });

        }
        //get mywishlist based on customerid
        [CustomAuthorize(UserRoles.Admin, UserRoles.User)]
        public IActionResult MyWishList()
        {
                return View();

        }

        [HttpGet]
        public async Task<IActionResult> MyWishList(int Customer_Id)
        {
            int? role = UserRole;
            int? userid = UserId;
            if (role == (int)UserRoles.Admin || (role == (int)UserRoles.User && userid == Customer_Id))
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
            /*TempData["AlertMessage"] = "You dont have access to this page.";*/
            return RedirectToAction("MyWishList", new { Customer_Id = userid });

        }
    
     





    }
}
