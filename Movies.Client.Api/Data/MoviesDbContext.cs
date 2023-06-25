using Microsoft.EntityFrameworkCore;
using Movies.Client.Api.Models;

namespace Movies.Client.Api.Data
{
    public class MoviesDbContext : DbContext
    {
        

        public MoviesDbContext(DbContextOptions<MoviesDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<MovieModel> Movies { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
