using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Movies.Admin.Api.Models;

namespace Movies.Admin.Api.Data
{
    public class AccountsDbContext : IdentityDbContext<UserModel>
    {
        public readonly IHttpContextAccessor httpContextAccessor;

        public AccountsDbContext(DbContextOptions<AccountsDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        //public DbSet<UserTokenModel> ApplicationUserTokens { get; set; }
        public DbSet<MovieModel> Movies { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
