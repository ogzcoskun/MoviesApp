using MediatR;
using Movies.Accounts.Api.Models.TokenModels;
using Movies.Accounts.Api.Models;
using Movies.Accounts.Api.Models.ActionModels;

namespace Movies.Accounts.Api.Commands
{
    public class LoginUserCommand : IRequest<ServiceResponse<TokenInfo>>
    {
        public LoginModel LoginInfo { get; set; }
    }
}
