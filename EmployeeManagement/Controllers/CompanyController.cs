using EmployeeManagement.DataAcessLayer;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EmployeeManagement.Controllers
{
    public class CompanyController : Controller
    {
        
        //Call the Connection String
    
        private readonly CompanyDAL _companyDAL;

        public CompanyController(CompanyDAL companyDAL)
        {
          
            this._companyDAL = companyDAL;
        }

        public IActionResult Index()
        {         
                List<CountryModel> countryData = _companyDAL.GetCountryData();
                ViewBag.countryData = new SelectList(countryData,"Id","Country");

            List<AreaModel> areaData = _companyDAL.GetAreaData();
            ViewBag.areaData = areaData;
            return View();
            
        }
          [HttpPost]
    public IActionResult IsCompanyNameAvailable(string companyName)
    {
        bool exists = _companyDAL.IsCompanyExists(companyName);
        return Json(!exists);
    }

        [HttpPost]
        public IActionResult SaveCompany(CompanyModel model)
        {
            if (ModelState.IsValid)
            {
                if (_companyDAL.SaveCompany(model, out string errorMessage))
                {
                    TempData["SuccessMessage"] = "Company saved successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    TempData["ErrorMessage"] = "Company saved Failed.";
                }
            }
            List<CountryModel> countryData = _companyDAL.GetCountryData();
            ViewBag.countryData = new SelectList(countryData, "Id", "Country");
            List<AreaModel> areaData = _companyDAL.GetAreaData();
            ViewBag.areaData = areaData;
            return View("Index", model);
        }
    }
}






  
