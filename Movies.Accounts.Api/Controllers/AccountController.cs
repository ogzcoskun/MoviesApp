using DotNetCore.CAP;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Movies.Accounts.Api.Commands;
using Movies.Accounts.Api.Models;
using Movies.Accounts.Api.Models.ActionModels;
using Movies.Accounts.Api.Services.AccountServices;

namespace Movies.Accounts.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly ICapPublisher _publisher;

        public AccountController(ILogger<AccountController> logger, IMediator mediator, ICapPublisher publisher)
        {
            _logger = logger;
            _mediator = mediator;

            _publisher = publisher;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel registration)
        {
            try
            {
                var response = new ServiceResponse<string>();

                if (!ModelState.IsValid)
                {
                    response.Success = false;
                    response.Message = "Model is Not valid please check your informations!!!";



                    return BadRequest(response);
                }

                response = await _mediator.Send(new RegisterUserCommand()
                {
                    Registration = registration
                });

                if (response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);

            }
            catch(Exception ex)
            {


                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginModel loginInfo)
        {

            try
            {

                if (ModelState.IsValid == false)
                {
                   
                    return BadRequest(new ServiceResponse<string>()
                    {
                        Success = false,
                        Message = "Model is not valid please try again",
                    });
                }

                var response = await _mediator.Send(new LoginUserCommand()
                {
                    LoginInfo = loginInfo
                });

                if (response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);

            }
            catch(Exception ex)
            {
                return BadRequest(new ServiceResponse<string>()
                {
                    Success = false,
                    Data = ex.Message,
                    Message = "Something went wrong while trying to Login!!!"
                });
            }


        }


    }
}
