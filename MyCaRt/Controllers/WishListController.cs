using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyCaRt.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MyCaRt.Controllers
{
    public class WishListController : Controller
    {
        public readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public WishListController(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUri"]);

        }


        [HttpPost]
        public async Task<IActionResult> AddToWishList(CartItemModel cartItem)
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
                    HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/WishList/AddToWishList", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content
                        var responseContent = await response.Content.ReadAsStringAsync();

                        // Deserialize the response to a JObject to access properties
                        var responseData = JObject.Parse(responseContent);

                        // Check if 'success' property is true
                        if (responseData["success"].ToObject<bool>())
                        {
                            return Json(new { success = true });
                        }
                        else
                        {
                            // Handle the case where success is false
                            return Json(new { success = false, message = "This product is already in your wishlist." });
                        }
                        // return Json(new { success = true });
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

    }
}
