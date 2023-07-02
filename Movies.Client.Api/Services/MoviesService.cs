using Amazon.Runtime.Internal.Util;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Movies.Client.Api.Config;
using Movies.Client.Api.Data;
using Movies.Client.Api.Models;
using Movies.Client.Api.Models.Responses;
using Movies.Client.Api.Models.Reviews;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Movies.Client.Api.Services
{
    public class MoviesService : IMoviesService
    {
        private readonly MoviesDbContext _sqlContext;
        private readonly IConfiguration _config;

        private readonly IConnectionMultiplexer _redisCon;
        private readonly IDatabase _cache;

        public MongoClient Client { get; }
        public IMongoDatabase DB { get; }

        private readonly ICapPublisher _capPublisher;

        public MoviesService(MoviesDbContext sqlContext, IConfiguration config, ICapPublisher capPublisher, IConnectionMultiplexer redisCon)
        {
            _sqlContext = sqlContext;

            _config = config;

            Client = new MongoClient(_config["MongoDbKey"]);
            DB = Client.GetDatabase("MoviesCluster");

            _capPublisher = capPublisher;

            _redisCon = redisCon;
            _cache = redisCon.GetDatabase();
        }

        public async Task<PagedResponse<List<MovieModel>>> GetAllMovies(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await _sqlContext.Movies
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToListAsync();
            var totalRecords = await _sqlContext.Movies.CountAsync();

            var response = new PagedResponse<List<MovieModel>>(pagedData, validFilter.PageNumber, validFilter.PageSize);

            response.Success = true;

            return response;
        }

        public async Task<ServiceResponse<ReviewModel>> AddComment(string userId, ReviewModel review)
        {
            try
            {

                var collection = DB.GetCollection<UserReviewsModel>("UserReviews");
                var user = (await collection.FindAsync(x => x.UserId == userId)).FirstOrDefaultAsync();


                review.ReviewId = Guid.NewGuid().ToString();


                var userToReplace = user.Result;

                

                if(userToReplace.Reviews == null)
                {
                    userToReplace.Reviews = new List<ReviewModel>();
                    userToReplace.Reviews.Add(review);
                }
                else
                {
                    var ratingCheck = userToReplace.Reviews.FirstOrDefault(x => x.MovieId == review.MovieId);
                    if(ratingCheck != null)
                    {
                        return new ServiceResponse<ReviewModel>()
                        {
                            Success = false,
                            Message = "You cant make more then one comment on a movie",
                            Data = ratingCheck                           
                        };
                    }




                    userToReplace.Reviews.Add(review);
                }

                await collection.ReplaceOneAsync(x => x.UserId == userId, userToReplace);

                var movieOnSql = await _sqlContext.Movies.FirstOrDefaultAsync(x => x.Id == review.MovieId);

                movieOnSql.RatingCount = movieOnSql.RatingCount + 1;
                movieOnSql.Rating = ((movieOnSql.Rating + review.Rating) / movieOnSql.RatingCount);

                await _sqlContext.SaveChangesAsync();


                return new ServiceResponse<ReviewModel>()
                {
                    Success = true,
                    Message = "Review Added",
                    Data = review
                };


            }
            catch (Exception ex)
            {
                return new ServiceResponse<ReviewModel>()
                {
                    Success = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<ServiceResponse<GetMoviesWithIdResponseModel>> GetMovieWithId(string userId, string movieId)
        {
            try
            {
                var collection = DB.GetCollection<UserReviewsModel>("UserReviews");
                var userOnMongo = (await collection.FindAsync(x => x.UserId == userId)).FirstOrDefaultAsync();

                var userReviewsOnMongo = userOnMongo.Result.Reviews;

                var review = new ReviewModel();

                if(userReviewsOnMongo != null)
                {
                    review = userReviewsOnMongo.FirstOrDefault(x => x.MovieId == movieId);
                }

                int rating;
                string comment;

                if(review.MovieId == null)
                {
                    rating = 0;
                    comment = "";
                }
                else
                {
                    rating = review.Rating;
                    comment = review.Comment;
                }


                var movie = await _sqlContext.Movies.FindAsync(movieId);

                if(movie == null)
                {
                    return new ServiceResponse<GetMoviesWithIdResponseModel>()
                    {
                        Success = false,
                        Message = "Given MovieId does not exist!!!"
                    };
                }

                

                var response = new ServiceResponse<GetMoviesWithIdResponseModel>()
                {
                    Success = true,
                    Message = "Movie.",
                    Data = new GetMoviesWithIdResponseModel()
                    {
                        Movie = movie,
                        UserRating = rating,
                        UserComment = comment
                    }
                };

                return response;

            }
            catch(Exception ex)
            {
                return new ServiceResponse<GetMoviesWithIdResponseModel>()
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResponse<MovieModel>> RecommendMovie(string userId, string ToEmail, string movieId, string personalMessage)
        {
            try
            {

                var movieToRecommend = await _sqlContext.Movies.FindAsync(movieId);

                if (movieToRecommend == null)
                {
                    return new ServiceResponse<MovieModel>()
                    {
                        Success =false,
                        Message = "Given movie Id does not exist!!!"
                    };
                }

                //Publish movie on rabbitmq so Admin api can handle it!!!

                

                var recommendation = new RecommendMovieModel()
                {
                    MovieId = movieId,
                    PersonalMessage = personalMessage,
                    ToEmail = ToEmail,
                    UserId = userId
                };


                var recommendationJson = JsonConvert.SerializeObject(recommendation);

                _capPublisher.Publish("UserRecommendAMovie", recommendationJson);

                return new ServiceResponse<MovieModel>()
                {
                    Success = true,
                    Message = "User recommend a movie..",
                    Data = movieToRecommend
                };

            }
            catch(Exception ex)
            {
                return new ServiceResponse<MovieModel>()
                {
                    Success =false,
                    Message = ex.Message
                };
            }
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



    }
}
