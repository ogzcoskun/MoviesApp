using Microsoft.AspNetCore.Identity;

namespace Movies.Admin.Api.Models
{
    public class UserModel :  IdentityUser
    {
        public string FullName { get; set; }
    }
}
