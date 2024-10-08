﻿using System.ComponentModel.DataAnnotations;

namespace MyCaRt.Models
{
    public class RegisterUserModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [RegularExpression(@"^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "eg:example@gmail.com.")]
        public string? Email { get; set; }

        [Required]
        [MinLength(8,ErrorMessage ="Password is too short minium length 8")]
        [MaxLength(15, ErrorMessage = "Password is too long maxium length 15")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).*$", ErrorMessage = "Atleast 1 capital and 1 lower and special character and one digit and 8-15 character.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? CPassword { get; set; }
    }
}
