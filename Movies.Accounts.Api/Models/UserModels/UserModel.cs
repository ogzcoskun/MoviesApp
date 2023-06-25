using Microsoft.AspNetCore.Identity;

namespace Movies.Accounts.Api.Models.UserModels
{
    public class UserModel : IdentityUser
    {
        public string FullName { get; set; }
    }
}
