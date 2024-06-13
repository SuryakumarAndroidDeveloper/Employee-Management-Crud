using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using MyCaRt.Controllers;
using Moq.Protected;
using MyCaRt.Models;

namespace MyCaRt.XUnitTests
{
    public class ProductCategoryTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly ProductCategoryController _controller;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;

        public ProductCategoryTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.SetupGet(x => x["ApiSettings:BaseUri"]).Returns("http://localhost");

            _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            _controller = new ProductCategoryController(_mockConfig.Object, _httpClient);
        }

        [Fact]
        public async Task CreateProductCategory_Post_Success()
        {
            var productCategory = new ProductCategoryModel { Category_Id = 1, Category_Name = "Electronics" };
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _controller.CreateProductCategory(productCategory);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("CreateProductCategory", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task CreateProductCategory_Post_Failure()
        {
            var productCategory = new ProductCategoryModel { Category_Id = 1, Category_Name = "Electronics" };
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _controller.CreateProductCategory(productCategory);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(productCategory, viewResult.Model);
        }

        [Fact]
        public async Task IsCategory_NameAvailable_Available()
        {
            var request = new ProductCategoryController.CategoryNameRequest { CategoryName = "Electronics" };
            var responseContent = JsonConvert.SerializeObject(new { Exists = true });
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _controller.IsCategory_NameAvailable(request);

            var jsonResult = Assert.IsType<JsonResult>(result);
            dynamic data = jsonResult.Value;
            Assert.True(data.Exists);
        }

        [Fact]
        public async Task IsCategory_NameAvailable_NotAvailable()
        {
            var request = new ProductCategoryController.CategoryNameRequest { CategoryName = "Electronics" };
            var responseContent = JsonConvert.SerializeObject(new { Exists = false });
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _controller.IsCategory_NameAvailable(request);

            var jsonResult = Assert.IsType<JsonResult>(result);
            dynamic data = jsonResult.Value;
            Assert.False(data.Exists);
        }
    }
}
