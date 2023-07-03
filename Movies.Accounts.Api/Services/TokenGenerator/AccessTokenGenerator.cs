using Microsoft.IdentityModel.Tokens;
using Movies.Accounts.Api.Data;
using Movies.Accounts.Api.Models.TokenModels;
using Movies.Accounts.Api.Models.UserModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Movies.Accounts.Api.Services.TokenGenerator
{
    public class AccessTokenGenerator
    {
        public AccountsDbContext _context { get; set; }
        public IConfiguration _config { get; set; }
        public UserModel _applicationUser { get; set; }

        public AccessTokenGenerator(AccountsDbContext context,
                                    IConfiguration config,
                                    UserModel applicationUser)
        {
            _config = config;
            _context = context;
            _applicationUser = applicationUser;
        }


        public UserTokenModel GetToken()
        {
            UserTokenModel userTokens = null;
            TokenInfo tokenInfo = null;

            
            if (_context.ApplicationUserTokens.Count(x => x.UserId == _applicationUser.Id) > 0)
            {
                
                userTokens = _context.ApplicationUserTokens.FirstOrDefault(x => x.UserId == _applicationUser.Id);

                
                if (userTokens.ExpireDate <= DateTime.Now)
                {
                    
                    tokenInfo = GenerateToken();

                    userTokens.ExpireDate = tokenInfo.ExpireDate;
                    userTokens.Value = tokenInfo.Token;

                    _context.ApplicationUserTokens.Update(userTokens);
                }
            }
            else
            {
                
                tokenInfo = GenerateToken();

                userTokens = new UserTokenModel();

                userTokens.UserId = _applicationUser.Id;
                userTokens.LoginProvider = "SystemAPI";
                userTokens.Name = _applicationUser.UserName;
                userTokens.ExpireDate = tokenInfo.ExpireDate;
                userTokens.Value = tokenInfo.Token;

                _context.ApplicationUserTokens.Add(userTokens);
            }

            _context.SaveChangesAsync();

            return userTokens;
        }


        public async Task<bool> DeleteToken()
        {
            bool ret = true;

            try
            {
                
                if (_context.ApplicationUserTokens.Count(x => x.UserId == _applicationUser.Id) > 0)
                {
                    UserTokenModel userTokens = userTokens = _context.ApplicationUserTokens.FirstOrDefault(x => x.UserId == _applicationUser.Id);

                    _context.ApplicationUserTokens.Remove(userTokens);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }


        private TokenInfo GenerateToken()
        {
            DateTime expireDate = DateTime.Now.AddHours(2);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Application:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _config["Application:Audience"],
                Issuer = _config["Application:Issuer"],
                Subject = new ClaimsIdentity(new Claim[]
                {
 
                    new Claim(ClaimTypes.NameIdentifier, _applicationUser.Id),
                    new Claim(ClaimTypes.Name, _applicationUser.UserName),
                    new Claim(ClaimTypes.Email, _applicationUser.Email)
                }),


                Expires = expireDate,

 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            TokenInfo tokenInfo = new TokenInfo();

            tokenInfo.Token = tokenString;
            tokenInfo.ExpireDate = expireDate;

            return tokenInfo;
        }
    }
}
