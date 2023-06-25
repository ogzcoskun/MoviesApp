using MediatR;
using Movies.Accounts.Api.Models;
using Movies.Accounts.Api.Models.ActionModels;

namespace Movies.Accounts.Api.Commands
{
    public class RegisterUserCommand : IRequest<ServiceResponse<string>> 
    {
        public RegisterModel Registration { get; set; }
    }
}
