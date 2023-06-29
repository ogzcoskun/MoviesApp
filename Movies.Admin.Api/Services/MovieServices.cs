using MongoDB.Driver;
using Movies.Admin.Api.Models;
using Movies.Admin.Api.Models.ReviewModels;

namespace Movies.Admin.Api.Services
{
    public class MovieServices : IMovieServices
    {
        private readonly IConfiguration _config;

        public MongoClient Client { get; }
        public IMongoDatabase DB { get; }

        public MovieServices(IConfiguration config)
        {
            _config = config;

            Client = new MongoClient(_config["MongoDbKey"]);
            DB = Client.GetDatabase("MoviesCluster");
        }

        public async Task<ServiceResponse<UserReviewsModel>> CreateUser(UserModel user)
        {
            try
            {

                var userReview = new UserReviewsModel()
                {
                    UserId = user.Id                 
                };

                var collection = DB.GetCollection<UserReviewsModel>("UserReviews");
                collection.InsertOne(userReview);

                return new ServiceResponse<UserReviewsModel>()
                {
                    Success = true,
                    Message = "User Created",
                    Data = userReview
                };


            }
            catch(Exception ex)
            {
                return new ServiceResponse<UserReviewsModel>()
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        
    }
}

