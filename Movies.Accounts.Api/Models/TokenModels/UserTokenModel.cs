using Microsoft.AspNetCore.Identity;

namespace Movies.Accounts.Api.Models.TokenModels
{
    public class UserTokenModel : IdentityUserToken<string>
    {
        public DateTime ExpireDate { get; set; }
    }
}
