using Movies.Admin.Api.Models;

namespace Movies.Admin.Api.Services.CacheServices
{
    public interface ICacheService
    {
        Task<ServiceResponse<List<RecommendMovieModel>>> GetRecommendations(string userId);
        Task<ServiceResponse<string>> SetRecommendation(RecommendMovieModel recommendation);
    }
}
