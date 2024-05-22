using Microsoft.AspNetCore.Mvc;
using StuDin.Database;
using StuDin.Models;

namespace StuDin.Controllers
{
    public class CountryController : Controller
    {
        public IActionResult Company()
        {
           // List<CountryModel> country = new List<CountryModel>();
          // DataRepository dataRepository = new DataRepository();
           // country = dataRepository.GetCountries();

            return View();
        }
    }
}
