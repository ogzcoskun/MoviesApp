using Movies.Client.Api.Config;
using Movies.Client.Api.Models;
using Movies.Client.Api.Models.Responses;
using Movies.Client.Api.Models.Reviews;

namespace Movies.Client.Api.Services
{
    public interface IMoviesService
    {
        Task<PagedResponse<List<MovieModel>>> GetAllMovies(PaginationFilter filter);
        Task<ServiceResponse<ReviewModel>> AddComment(string userId, ReviewModel review);
        Task<ServiceResponse<GetMoviesWithIdResponseModel>> GetMovieWithId(string userId, string movieId);
        Task<ServiceResponse<MovieModel>> RecommendMovie(string userId, string ToEmail, string movieId, string personalMessage);
        Task<ServiceResponse<List<RecommendMovieModel>>> GetRecommendations(string userId);
    }
}
