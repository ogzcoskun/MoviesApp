using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Movies.Accounts.Api.Models.TokenModels;
using Movies.Accounts.Api.Models.UserModels;

namespace Movies.Accounts.Api.Data
{
    public class AccountsDbContext : IdentityDbContext<UserModel>
    {
        public readonly IHttpContextAccessor httpContextAccessor;

        public AccountsDbContext(DbContextOptions<AccountsDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public DbSet<UserTokenModel> ApplicationUserTokens { get; set; }
       


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
