using EmployeeManagement.DataAcessLayer;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagement.Controllers
{
    public class EmployeController : Controller
    {
        

        private readonly EmployeDAL _employeDAL;

        public EmployeController(EmployeDAL employeDAL)
        {
        
            this._employeDAL = employeDAL;
        }
        public IActionResult create()
        {
            List<CompanyModel> companyData = _employeDAL.GetCompanyNameData();
            ViewBag.companyData = new SelectList(companyData, "Id", "CompanyName");
            return View();
        }
        [HttpPost]
        public IActionResult IsEmailAvailable(string email)
        {
            bool exists = _employeDAL.IsEmailExists(email);
            return Json(!exists);
        }

        [HttpPost]
        public IActionResult IsMobileAvailable(string mobile)
        {
            bool exists = _employeDAL.IsMobileExists(mobile);
            return Json(!exists);
        }



        [HttpPost]
        public IActionResult Insert(EmployeModel employeModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                
                    bool isInserted = _employeDAL.InsertEmployee(employeModel);
                    if (isInserted)
                    {
                        Console.WriteLine("User Inserted Successfully");
                        TempData["SuccessMessage"] = "Employee saved successfully.";
                        return RedirectToAction("ListEmployee");
                    }
                    else
                    {
                        Console.WriteLine("User Insertion Failed");
                        return View(employeModel);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return View(employeModel);
                }
            }
            else
            {
                return View(employeModel);
            }
        }



        [HttpGet]
        public IActionResult ListEmployee()
        {
            var activeEmployees = _employeDAL.GetActiveEmployees();

            // Pass the list of active employees to the view
            return View(activeEmployees);

            
        }











      /*  [HttpGet]
        public IActionResult ListEmployee()
        {
            List<EmployeModel> employees = _employeDAL.GetAllEmployees();
            return View(employees);
        }*/
        [HttpGet]
        public IActionResult Index()
        {
         
            return View();
        }


      [HttpPost]
        public IActionResult DeactivateEmployee(int id)
        {
            _employeDAL.DeactivateEmployee(id);
            // Optionally, you can redirect to another page or return a JSON response.
            return RedirectToAction("ListEmployee", "Employe");
        }

       
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                bool isDeleted = _employeDAL.DeleteEmployee(id);
                if (isDeleted)
                {
                    Console.WriteLine("Employee Deleted Successfully");
                }
                else
                {
                    Console.WriteLine("Failed to delete employee");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("ListEmployee"); // Redirect to the employee list page after deletion

            
        }

        public IActionResult Delete()
        {
            return View() ; 
        }

        public IActionResult Update()
        {
            return View();
        }




        [HttpGet]
        public IActionResult Edit(int id)
        {
            var employee = _employeDAL.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            List<CompanyModel> companyData = _employeDAL.GetCompanyNameData();
            ViewBag.companyData = new SelectList(companyData, "Id", "CompanyName");
            return View(employee);
        }




        [HttpPost]
        public IActionResult Edit(EmployeModel employeModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool isUpdated = _employeDAL.UpdateEmployee(employeModel);
                    if (isUpdated)
                    {
                        Console.WriteLine("Employee Updated Successfully");
                        TempData["SuccessMessage"] = "Employee saved successfully.";
                        return RedirectToAction("ListEmployee");
                    }
                    else
                    {
                        Console.WriteLine("Failed to Update Employee");
                        return View(employeModel);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return View(employeModel);
                }
            }
            else
            {
                return View(employeModel);
            }
        }

        [HttpGet]
        public IActionResult ViewEmployee(int id)
        {
            var employee = _employeDAL.GetEmpById(id);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }
            return View(employee);
        }
        [HttpPost]
        public IActionResult DeleteSelectedEmployees([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest("No IDs provided for deletion.");
            }

            try
            {
                _employeDAL.DeleteEmployees(ids);
                TempData["SuccessMessage"] = "Selected employees deleted successfully.";
                return RedirectToAction("ListEmployee");
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }




    }
}
