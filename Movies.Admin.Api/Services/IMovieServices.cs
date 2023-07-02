using Movies.Admin.Api.Models;
using Movies.Admin.Api.Models.ReviewModels;

namespace Movies.Admin.Api.Services
{
    public interface IMovieServices
    {
        public Task<ServiceResponse<UserReviewsModel>> CreateUser(UserModel user);
        public Task<ServiceResponse<string>> SendEmail(RecommendMovieModel recommendation);
    }
}
