using System.ComponentModel.DataAnnotations;

namespace MyCaRt.Models
{
    public class ResetPasswordModel
    {
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password is too short minium length 8")]
        [MaxLength(15, ErrorMessage = "Password is too long maxium length 15")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).*$", ErrorMessage = "Atleast 1 capital and 1 lower and special character and one digit and 8-15 character.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string CPassword { get; set; }









    }
}
