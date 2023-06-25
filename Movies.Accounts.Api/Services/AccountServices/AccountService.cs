using Microsoft.AspNetCore.Identity;
using Movies.Accounts.Api.Data;
using Movies.Accounts.Api.Models;
using Movies.Accounts.Api.Models.ActionModels;
using Movies.Accounts.Api.Models.TokenModels;
using Movies.Accounts.Api.Models.UserModels;
using Movies.Accounts.Api.Services.TokenGenerator;

namespace Movies.Accounts.Api.Services.AccountServices
{
    public class AccountService : IAccountService
    {

        private readonly AccountsDbContext _context;
        private readonly IConfiguration _config;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountService(AccountsDbContext context,
                              IConfiguration configuration,
                              SignInManager<UserModel> signInManager,
                              UserManager<UserModel> userManager,
                              RoleManager<IdentityRole> roleManager)
        {

            _context = context;
            _config = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;

        }

        public async Task<ServiceResponse<TokenInfo>> Login(LoginModel loginInfo)
        {
            try
            {

                UserModel user = await _userManager.FindByNameAsync(loginInfo.Email);

                //Kullanıcı var ise;
                if (user == null)
                {
                    return new ServiceResponse<TokenInfo>()
                    {
                        Success = false,
                        Message = "Couldn't find user please check email!!!"
                    };
                }

                SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, loginInfo.Password, false, false);
                //Kullanıcı adı ve şifre kontrolü
                if (signInResult.Succeeded == false)
                {
                    return new ServiceResponse<TokenInfo>()
                    {
                        Success = false,
                        Message = "Username or password is wrong"
                    };
                }


                UserModel applicationUser = _context.Users.FirstOrDefault(x => x.Id == user.Id);

                AccessTokenGenerator accessTokenGenerator = new AccessTokenGenerator(_context, _config, applicationUser);
                UserTokenModel userTokens = accessTokenGenerator.GetToken();

                return new ServiceResponse<TokenInfo>()
                {
                    Success = true,
                    Message = "User successfully logged in",
                    Data = new TokenInfo()
                    {
                        Token = userTokens.Value,
                        ExpireDate = userTokens.ExpireDate
                    }
                };


            }
            catch(Exception ex)
            {
                return new ServiceResponse<TokenInfo>()
                {
                    Success = false,
                    Message = "Something went wrong while trying to login",

                };
            }
        }

        public async Task<ServiceResponse<UserModel>> Register(RegisterModel registration)
        {
            try
            {

                var response = new ServiceResponse<UserModel>();

                UserModel existsUser = await _userManager.FindByNameAsync(registration.Email);

                if (existsUser != null)
                {
                    return new ServiceResponse<UserModel>()
                    {
                        Success = false,
                        Message = "User already exist please try to login!!!"
                    };
                }

                UserModel user = new UserModel();

                user.FullName = registration.FullName;
                user.Email = registration.Email.Trim();
                user.UserName = registration.Email.Trim();

                //Kullanıcı oluşturulur.
                IdentityResult result = await _userManager.CreateAsync(user, registration.Password.Trim());

                if (!result.Succeeded)
                {
                    return new ServiceResponse<UserModel>()
                    {
                        Success = false,
                        Message = "Passwords must have at least one non alphanumeric, one number and one uppercase letter!!!"
                    };
                }

                //Kullanıcı oluşturuldu ise
                if (result.Succeeded)
                {
                    bool roleExists = await _roleManager.RoleExistsAsync(_config["Roles:User"]);

                    if (!roleExists)
                    {
                        IdentityRole role = new IdentityRole(_config["Roles:User"]);
                        role.NormalizedName = _config["Roles:User"];

                        _roleManager.CreateAsync(role).Wait();
                    }

                    //Kullanıcıya ilgili rol ataması yapılır.
                    _userManager.AddToRoleAsync(user, _config["Roles:User"]).Wait();

                    response.Success = true;
                    response.Message = "User created...";
                    response.Data = user;
                    
                }
                else
                {
                    response.Success = false;
                    response.Message = "Something went wrong while trying to register user";
                }

                return response;


            }
            catch(Exception ex)
            {
                return new ServiceResponse<UserModel>()
                {
                    Success = false,
                    Message = "Something went wrong while trying to register user!!!",
                };
            }
        }
    }
}
