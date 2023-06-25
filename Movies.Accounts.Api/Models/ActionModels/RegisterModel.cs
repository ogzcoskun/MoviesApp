using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Movies.Accounts.Api.Models.ActionModels
{
    public class RegisterModel
    {
        [Required]
        [StringLength(60)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required!!!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required!!!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords doesn't match.")]
        public string ConfirmPassword { get; set; }
    }
}
