using EmployeeManagement.DataAcessLayer;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{




    public class CompanyModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Contact Person is required.")]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }


        [Required(ErrorMessage = "Please select at least one area")]
        public List<int> SelectedAreas { get; set; }

        public CompanyModel()
        {
            SelectedAreas = new List<int>();
        }


        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [RegularExpression(@"^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [Phone(ErrorMessage = "Invalid Phone Number.")]
        [RegularExpression(@"^[6789]\d{9}$", ErrorMessage = "Number start with (6,7,8,9) and 9 numbers")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public String Country { get; set; }



    }
}








    

  


