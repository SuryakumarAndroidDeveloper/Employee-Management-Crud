using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class EmployeModel
    {

   
    [Key]
        public int Id { get; set; }
  

        [Required(ErrorMessage = "Company Name is Required.")]
        public String CompanyName { get; set; }

        [Required(ErrorMessage = "Name is Required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is Required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [RegularExpression(@"^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile is Required.")]
        [Phone(ErrorMessage = "Invalid Phone Number.")]
        [RegularExpression(@"^[6789]\d{9}$", ErrorMessage = "Number start with (6,7,8,9) and 9 numbers")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Address is Required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is Required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Pincode is Required.")]
        [Range(100000, 999999, ErrorMessage = "Invalid Pincode.")]
        public int Pincode { get; set; }
    }
}
