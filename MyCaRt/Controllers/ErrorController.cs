using Microsoft.AspNetCore.Mvc;

namespace MyCaRt.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            // Handle different error codes here
            if (statusCode == 404)
            {
                // Return the 404 error view
                return View("NotFound");
            }
            // Handle other errors accordingly
            return View("Error"); // Generic error view
        }
    }
}
