using System.ComponentModel.DataAnnotations;

namespace MyCaRt.Models
{
    public class CustomerModel
    {
        [Key]
        public int Customer_Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string Customer_FName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string Customer_LName { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Customer_Gender { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Customer_Email { get; set; }

        [Required(ErrorMessage = "Mobile is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Customer_Mobile { get; set; }

        [Required(ErrorMessage = "Please select at least one area")]
        public List<string> SelectedAreas { get; set; }

        public CustomerModel()
        {
            SelectedAreas = new List<string>();
        }

        public string? Customer_InterestedCategory { get; set; }


        public string? FilePath { get; set; }
        public string? ImageName { get; set; }


    }



    }
