using MediatR;
using Movies.Accounts.Api.Models.TokenModels;
using Movies.Accounts.Api.Models;
using Movies.Accounts.Api.Services.AccountServices;

namespace Movies.Accounts.Api.Commands.Handlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, ServiceResponse<TokenInfo>>
    {
        private readonly IAccountService _accountService;

        public LoginUserCommandHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<ServiceResponse<TokenInfo>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var response = await _accountService.Login(request.LoginInfo);

                return response;

            }
            catch(Exception ex)
            {
                return new ServiceResponse<TokenInfo>()
                {
                    Success = false,
                    Message = "Something went wrong while login!!!",
                    
                };
            }
        }
    }
}
