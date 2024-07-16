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


    }
}
