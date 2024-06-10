using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyCaRt.Models;
using Newtonsoft.Json;
using System.Text;

namespace MyCaRt.Controllers
{
    public class CartController : Controller
    {
        public readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public CartController(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUri"]);

        }

        //add the product to cart

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
                    HttpResponseMessage response1 = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Customer/GetAllCustomer_Name");

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
                    HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Cart/AddToCart", content);

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
        //customer cart page
        public async Task<IActionResult> Customer_Cart()
        {


            List<Customer_NameModel> customerData = new List<Customer_NameModel>();

            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Customer/GetAllCustomer_Name");

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


            return View();
        }

//get cart based on customerid

        [HttpGet]
        public async Task<IActionResult> GetCartByCustomer(int Customer_Id)
        {
            List<DisplayCartModel> cartData = new List<DisplayCartModel>();

            HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Cart/GetCartByCustomer?customerId={Customer_Id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                cartData = JsonConvert.DeserializeObject<List<DisplayCartModel>>(data);
            }
            else
            {
                return BadRequest("Failed to load cart data.");
            }

            return Json(cartData);
        }
//update the quantity of the cartitem based on cartitem id
        [HttpPost]
        public async Task<IActionResult> UpdateCartItemQuantity(int cartItemId, int newQuantity)
        {
            var requestData = new { CartItemId = cartItemId, NewQuantity = newQuantity };
            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Cart/UpdateCartItemQuantity?cartItemId={cartItemId},newQuantity={newQuantity}", content);

            if (response.IsSuccessStatusCode)
            {
                return Ok("Quantity updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update quantity.");
            }
        }

//delete the cartitem based on cartitem id
        [HttpPost]
        public async Task<IActionResult> DeleteCartItem(int cartItemId)
        {
            var requestData = new { CartItemId = cartItemId };
            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Cart/DeleteCartItem?cartItemId={cartItemId}", content);

            if (response.IsSuccessStatusCode)
            {
                return Ok("Item deleted successfully.");
            }
            else
            {
                return BadRequest("Failed to delete item.");
            }
        }



    }
}
