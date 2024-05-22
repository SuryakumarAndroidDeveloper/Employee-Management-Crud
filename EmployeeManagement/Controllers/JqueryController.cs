using EmployeeManagement.DataAcessLayer;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagement.Controllers
{

    [Route("Jquery/[action]")]
    public class JqueryController : Controller
    {
        private readonly EmployeDAL _employeDAL;

        public JqueryController(EmployeDAL employeDAL)
        {
            _employeDAL = employeDAL;
        }
        

        
        public IActionResult Edit(int id)
        {
            var employee = _employeDAL.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            List<CompanyModel> companyData = _employeDAL.GetCompanyNameData();
            ViewBag.companyData = new SelectList(companyData, "Id", "CompanyName");
            return View();
            
        }

        [HttpGet]
        public IActionResult ModifyEmployeeData(int id)
        {
            var employee = _employeDAL.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            List<CompanyModel> companyData = _employeDAL.GetCompanyNameData();
            ViewBag.companyData = new SelectList(companyData, "Id", "CompanyName");
            return Json(employee);
        }

        [HttpPost]
        public IActionResult Edit(EmployeModel employeModel)
        {
            if (ModelState.IsValid)
            {
                bool isUpdated = _employeDAL.UpdateEmployee(employeModel);
                if (isUpdated)
                {
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }



        [HttpPost]
        public IActionResult Edit1(EmployeModel employeModel)
        {
            if (ModelState.IsValid)
            {
                bool isUpdated = _employeDAL.InsertEmployee(employeModel);
                if (isUpdated)
                {
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }




        public IActionResult ListEmployee()
        {
            var employees = _employeDAL.GetActiveEmployees();
            return View(employees);
        }


       

        public IActionResult Create()
        {
            List<CompanyModel> companyData = _employeDAL.GetCompanyNameData();
            ViewBag.companyData = new SelectList(companyData, "Id", "CompanyName");
            return View();
        }
    }
}
