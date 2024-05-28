using EmployeeManagement.Models;

namespace EmployeeManagement.Interfaces
{
    public interface IStudentRepository
    {
         Student GetStudentById(int StudentId);
        List<Student> GetAllStudent();
    }
}
