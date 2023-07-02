using Microsoft.AspNetCore.Identity;

namespace Movies.Admin.Api.Models
{
    public class UserTokenModel : IdentityUserToken<string>
    {
        public DateTime ExpireDate { get; set; }
    }
}
