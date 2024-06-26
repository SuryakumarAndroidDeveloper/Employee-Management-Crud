using Microsoft.AspNetCore.Mvc;
using MyCaRt.Helper;
using MyCaRt.Models;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace MyCaRt.Controllers
{
    public class PaymentController : Controller
    {
        public readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public PaymentController(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUri"]);

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("StorePayment")]
        public async Task<IActionResult> StorePayment([FromBody]PaymentModel paymentModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid payment data.");
            }

            var encryptedPaymentModel = new PaymentModel
            {
                //CustomerId = paymentModel.CustomerId,
                FullName = EncryptionHelper.EncryptString(paymentModel.FullName),
                Email = EncryptionHelper.EncryptString(paymentModel.Email),
                Address = EncryptionHelper.EncryptString(paymentModel.Address),
                City = EncryptionHelper.EncryptString(paymentModel.City),
                State = EncryptionHelper.EncryptString(paymentModel.State),
                ZipCode = EncryptionHelper.EncryptString(paymentModel.ZipCode),
                CardName = EncryptionHelper.EncryptString(paymentModel.CardName),
                CardNumber = EncryptionHelper.EncryptString(paymentModel.CardNumber),
                ExpMonth = EncryptionHelper.EncryptString(paymentModel.ExpMonth),
                ExpYear = EncryptionHelper.EncryptString(paymentModel.ExpYear),
                CVV = EncryptionHelper.EncryptString(paymentModel.CVV)
            };

            var json = JsonConvert.SerializeObject(encryptedPaymentModel);

            // Create StringContent from JSON
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // PostAsync to your API
            HttpResponseMessage response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Payment/AddPayment", content);
            
            if (response.IsSuccessStatusCode)
            {
                // return Ok("Payment successfully!");
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);
                if (result.ContainsKey("paymentId"))
                {
                    return Ok(new { PaymentId = result["paymentId"] });
                }
                else
                {
                    return BadRequest("PaymentId not found in the response.");
                }

            }
            else
            {
                return BadRequest("Failed to Payment.");
            }
        }
    }
}
