using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using MyCaRt.Models;
using Newtonsoft.Json;
using System.Text;
using static MyCaRt.Enum.@enum;

namespace MyCaRt.Controllers
{
    [CustomAuthorize(UserRoles.Admin, UserRoles.User)]
    public class OrderController : Controller
    {
        public readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public OrderController(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUri"]);

        }
        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderProductModel orderRequest)
        {
            try
            {
                Console.WriteLine("Received order request:");
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(orderRequest));
                if (orderRequest == null)
                {
                    return BadRequest("Invalid order data.");
                }


                var orderProducts = orderRequest.OrderProducts;
                //var totalPrice = orderRequest.TotalPrice;
                // Serialize orderProducts to JSON
                var json = JsonConvert.SerializeObject(orderRequest);

                // Create StringContent from JSON
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // PostAsync to your API
                HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Order/PlaceOrder", content);

                if (response.IsSuccessStatusCode)
                {
                    return Ok("Order placed successfully!");
                }
                else
                {
                    return BadRequest("Failed to place the order.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as per your application's error handling strategy
                return BadRequest("Failed to place the order.");
            }
        }
        //buy again the product
        [HttpPost]
        public async Task<IActionResult> BuyAgainOrder([FromBody] BugAgainOrderModel orderData)
        {
            try
            {
                if (orderData == null)
                {
                    return BadRequest("Invalid order data.");
                }

                var json = JsonConvert.SerializeObject(orderData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Order/BuyAgainOrder", content);

                if (response.IsSuccessStatusCode)
                {
                    return Ok("Order placed successfully.");
                }
                else
                {
                    TempData["AlertBugAgainMessage"] = "Failed to Order this Product.";
                    return RedirectToAction("MyOrders");
                }
            }
            catch (Exception ex)
            {
                TempData["AlertBugAgainMessage"] = "Failed to place the order.";
                return RedirectToAction("MyOrders");
            }
        }

        //Cancel the Order based on the Orderid

        [HttpPost]
        [CustomAuthorize(UserRoles.Admin, UserRoles.User)]
        public async Task<IActionResult> CancelOrder(int orderId)

        {
                HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Order/CancelOrderById?orderId={orderId}", null);

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertCancelMessage"] = "Order Canceled!";
                    return RedirectToAction("MyOrders", "Customer");


                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact support.");
                    return RedirectToAction("ListOfCustomers");
                }
            }

        //display the fullorderdetails

        [CustomAuthorize(UserRoles.Admin, UserRoles.User)]
        public async Task<IActionResult> ListOfOrderDetails()
        {
            List<ListOfOrdersModel> orders = new List<ListOfOrdersModel>();

            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Order/GetAllOrders");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                orders = JsonConvert.DeserializeObject<List<ListOfOrdersModel>>(data);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact the administrator.");
            }

            return View(orders);

        }

        //get the orderdetails based on the orderid
        [HttpGet]
        [CustomAuthorize(UserRoles.Admin)]
        public async Task<IActionResult> EditFullOrder()
        {
            List<ListOfOrdersModel> orders = new List<ListOfOrdersModel>();

            HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Order/GetAllOrders");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                orders = JsonConvert.DeserializeObject<List<ListOfOrdersModel>>(data);            
            }
            else
            {
                return null;
            }
            return View(orders);
        }


        [HttpPost]
        [CustomAuthorize(UserRoles.Admin)]
        public async Task<IActionResult> UpdateFullOrders(List<ListOfOrdersModel> orders)
        {
            if (orders == null || !orders.Any())
            {
                return BadRequest("No orders to update.");
            }
            var json = JsonConvert.SerializeObject(orders);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Make the POST request to update customer details
            HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Order/UpdateFullOrderDetails", content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                TempData["SavedChangesFullDetails"] = "Updated Successfully";
                return RedirectToAction("EditFullOrder");
            }
            else
            {
                TempData["ErrorFullDetails"] = "Failed to update orders.";
                return View("Error");
            }
        }



        //edit the orderdetails cshtml page
        [CustomAuthorize(UserRoles.Admin, UserRoles.User)]
        public IActionResult EditOrder()
        {
            return View();
        }

        //get the orderdetails based on the orderid
        [HttpGet]
        [CustomAuthorize(UserRoles.Admin)]
        public async Task<IActionResult> EditOrder(int orderId)
        {
            ListOfOrdersModel order = null;

                HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Order/GetOrderById?orderId={orderId}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(data);
                order = JsonConvert.DeserializeObject<ListOfOrdersModel>(data);

                return View(order);
            }
            else
            {
                return null;
            }
        }
        //update the orderdetails based on the orderid
        [HttpPost]
        [CustomAuthorize(UserRoles.Admin)]
        public async Task<IActionResult> EditOrder(int orderId, ListOfOrdersModel order)
        {

                // Serialize the customer object to JSON
                var json = JsonConvert.SerializeObject(order);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Make the POST request to update customer details
                HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Order/UpdateOrderDetails?orderId={orderId}", content);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {             
                    TempData["SavedChanges"] = "Order Updated Successfully";
                    return RedirectToAction("ListOfOrderDetails");
                }
                else
                {
                    // If there's an error, add model error and return the view
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response Status Code: {response.StatusCode}");
                    Console.WriteLine($"Response Content: {responseContent}");
                    ModelState.AddModelError(string.Empty, $"Server error: {responseContent}");
                    return View(order);

                }
       
        }
        //Delete the orderdetails based on the orderid




    }
}
