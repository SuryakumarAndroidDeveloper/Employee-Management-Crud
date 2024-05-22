using Microsoft.AspNetCore.Mvc;
using StuDin.Database;
using StuDin.Models;

namespace StuDin.Controllers
{
    public class CompanyController : Controller
    {
        public readonly DataRepository _dataRepository;

        public CompanyController(DataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            List<CompanyModel> companies = new List<CompanyModel>();

            try
            {
                companies = _dataRepository.GetAll();
                
            }
            catch (Exception ex)
            {

                TempData["errorMessage"] = "Error has be found";
            }

            return View(companies);
        }
        
        public IActionResult insert()
        {
            return View();
                }
        public IActionResult list()
        {
            return View();
        }

        [HttpPost]
        public IActionResult insert123( CompanyModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["errorMessage"] = "Model Data is Invalid";
            }
            bool result = _dataRepository.insert123(model);
            if (!result)
            {
                TempData["errorMessage"] = "Unable To Save Data";
                
            }
            TempData["successMessage"] = "Saved Sucess";
            return RedirectToAction("list");
        }
    }
}
