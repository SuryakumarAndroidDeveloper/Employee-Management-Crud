using EmployeeManagement.DataAcessLayer;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        public IActionResult IsEmailAvailable(string email)
        {
            bool exists = _companyDAL.IsEmailExists(email);
            return Json(!exists);
        }
        [HttpPost]
        public IActionResult IsPhoneNumberAvailable(string phoneNumber)
        {
            bool exists = _companyDAL.IsPhoneNumberExists(phoneNumber);
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

        [HttpGet]
        public IActionResult ListCompany()
        {
            List<CompanyModel> companies = _companyDAL.GetAllCompanyWithAreas();
            return View(companies);
           
        }



        [HttpGet]

        [HttpGet]
        public IActionResult EditCompany(int id)
        {
            var company = _companyDAL.GetCompanyById(id);
            if (company == null)
            {
                return NotFound();
            }
            List<CountryModel> countryData = _companyDAL.GetCountryData();
            ViewBag.countryData = new SelectList(countryData, "Id", "Country");

            List<AreaModel> areaData = _companyDAL.GetAreaData();
            ViewBag.areaData = areaData;

            return View(company);
        }

        [HttpPost]
        public IActionResult EditCompany(CompanyModel companyModel)
        {

            if (ModelState.IsValid)
            {
                if (_companyDAL.UpdateCompany (companyModel, out string errorMessage))
                {
                    TempData["SuccessMessage"] = "Company updated successfully.";
                    return RedirectToAction("ListCompany");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }
            List<CountryModel> countryData = _companyDAL.GetCountryData();
            ViewBag.countryData = new SelectList(countryData, "Id", "Country");

            List<AreaModel> areaData = _companyDAL.GetAreaData();
            ViewBag.areaData = areaData;
            return View(companyModel);
        }








    }
}






  
