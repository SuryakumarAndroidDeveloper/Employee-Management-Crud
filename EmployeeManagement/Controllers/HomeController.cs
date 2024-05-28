using EmployeeManagement.DataAcessLayer;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        //Create a reference variable of IStudentRepository
        private readonly IStudentRepository? _repository = null;
     
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IStudentRepository repository)
        {
            _logger = logger;
           _repository = repository;
        }

        //without dependencyinjection how it works 
        /*      public JsonResult Index()
              {
                  StudentRepository repository = new StudentRepository();
                  List<Student> allStudentDetails = repository.GetAllStudent();
                  return Json(allStudentDetails);
              }
              public JsonResult GetStudentDetails(int Id)
              {
                  StudentRepository repository = new StudentRepository();
                  Student studentDetails = repository.GetStudentById(Id);
                  return Json(studentDetails);
              }*/

        //with dependency injection how it works
  /*      public JsonResult Index()
        {
            List<Student>? allStudentDetails = _repository?.GetAllStudent();
            _repository.GetStudentById(7);
            return Json(allStudentDetails);
        }
        public JsonResult GetStudentDetails(int Id)
        {
            Student? studentDetails = _repository?.GetStudentById(Id);
            return Json(studentDetails);
        }*/

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
