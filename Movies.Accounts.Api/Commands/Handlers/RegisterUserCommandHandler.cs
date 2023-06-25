using MediatR;
using Movies.Accounts.Api.Events.PublishEvents;
using Movies.Accounts.Api.Models;
using Movies.Accounts.Api.Services.AccountServices;

namespace Movies.Accounts.Api.Commands.Handlers
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ServiceResponse<string>>
    {
        private readonly IAccountService _accountService;
        private readonly IPublishEventsService _publishEvents;

        public RegisterUserCommandHandler(IAccountService accountService, IPublishEventsService publishEvents)
        {
            _accountService = accountService;
            _publishEvents = publishEvents;
        }

        public async Task<ServiceResponse<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            try
            {

                //Create User on Sql server
                var response = new ServiceResponse<string>();

                var createUser = await _accountService.Register(request.Registration);

                // if something went wrong just return error
                if (!createUser.Success)
                {
                    response.Success = createUser.Success;
                    response.Message = createUser.Message;

                    return response;
                }

                // After user created, User Registered Event will be published and the api that create the users movie lists on mongo db will receive it to create it too

                await _publishEvents.UserRegisteredEvent(createUser.Data);

                response.Success = true;
                response.Message = "User Created";
                return response;

                //if user created on sql server now create user Movies List on mongo db


            }
            catch(Exception ex)
            {
                return new ServiceResponse<string>()
                {
                    Data = ex.Message,
                    Message = "Something went wrong while trying to register user",
                    Success = false
                };
            }
        }
    }
}
