using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StuDin.Models
{
    public class CompanyModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [Required]
        [DisplayName("Contact Person")]
        public string ContactPerson { get; set;}
        [Required]
        [DisplayName("Email")]
        public string Email { get; set;}
        [Required]
        [DisplayName("Phone Number")]
        public double PhoneNumber { get; set;}
        [Required]
        [DisplayName("Address")]
        public string Address { get; set;}
        [Required]
        [DisplayName("Country")]
        public string Country { get; set;}
    }
}
