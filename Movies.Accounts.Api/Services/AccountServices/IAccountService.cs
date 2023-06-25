using Movies.Accounts.Api.Models;
using Movies.Accounts.Api.Models.ActionModels;
using Movies.Accounts.Api.Models.TokenModels;
using Movies.Accounts.Api.Models.UserModels;

namespace Movies.Accounts.Api.Services.AccountServices
{
    public interface IAccountService
    {
        Task<ServiceResponse<UserModel>> Register(RegisterModel registration);
        Task<ServiceResponse<TokenInfo>> Login(LoginModel loginInfo);
    }
}
