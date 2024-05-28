using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CRUDApiApplication.Models
{
    public class EmployeModel
    {
        [Key]
        public int Id { get; set; }


        [Required]
        [DisplayName("CompanyName")]
        public string? CompanyName { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Mobile { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        public string? City { get; set; }

        [Required]
        public int Pincode { get; set; }
    }
}
