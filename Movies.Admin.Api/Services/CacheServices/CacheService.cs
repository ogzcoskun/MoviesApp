using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Movies.Admin.Api.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Movies.Admin.Api.Services.CacheServices
{
    public class CacheService : ICacheService
    {

        private readonly IConnectionMultiplexer _redisCon;
        private readonly IDatabase _cache;

        public CacheService(IConnectionMultiplexer redisCon)
        {

            _redisCon = redisCon;
            _cache = redisCon.GetDatabase();

        }

        public async Task<ServiceResponse<List<RecommendMovieModel>>> GetRecommendations(string userId)
        {
            try
            {

                var userMails = await _cache.StringGetAsync(userId);

                if (userMails.HasValue == false)
                {
                    return new ServiceResponse<List<RecommendMovieModel>>()
                    {
                        Success = true,
                        Message = "User has not recommend a movie yet!!!"
                    };
                }

               
                var recommList = JsonConvert.DeserializeObject<List<RecommendMovieModel>>(userMails);
                

                return new ServiceResponse<List<RecommendMovieModel>>()
                {
                    Success = true,
                    Message = "User Recommendations...",
                    Data = recommList
                };

            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<RecommendMovieModel>>()
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }


        public async Task<ServiceResponse<string>> SetRecommendation(RecommendMovieModel recommendation)
        {
            try
            {

                var userRecomms = await GetRecommendations(recommendation.UserId);

                if (!userRecomms.Success)
                {
                    return new ServiceResponse<string>()
                    {
                        Success = false,
                        Message = "Something went wrong!!!"
                    };
                }


                if (userRecomms.Success && userRecomms.Data == null)
                {
                    //var cacheInfo = JsonConvert.SerializeObject(recommendation);

                    var cacheList = new List<RecommendMovieModel>();
                    cacheList.Add(recommendation);

                    var cacheInfo = JsonConvert.SerializeObject(cacheList);

                    var respData = await _cache.StringSetAsync(recommendation.UserId, cacheInfo);

                    if (respData)
                    {
                        return new ServiceResponse<string>()
                        {
                            Success = true,
                            Message = "User Created and then set"
                        };
                    }

                    return new ServiceResponse<string>()
                    {

                        Success = false,
                        Message = "Something went wrong!!!"

                    };
                }

                var mailListToAdd = userRecomms.Data;
                mailListToAdd.Add(recommendation);

                var jsonMailList = JsonConvert.SerializeObject(mailListToAdd);

                var respData2 = await _cache.StringSetAsync(recommendation.UserId, jsonMailList);

                return new ServiceResponse<string>()
                {
                    Success = true,
                    Message = "Mail cached.."
                };


            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>()
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
