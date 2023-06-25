using System.ComponentModel.DataAnnotations;

namespace Movies.Accounts.Api.Models.ActionModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is requeired!!!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
