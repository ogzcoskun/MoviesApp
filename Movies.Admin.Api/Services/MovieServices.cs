using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MongoDB.Driver;
using Movies.Admin.Api.Models;
using Movies.Admin.Api.Models.ReviewModels;
using MailKit.Net.Smtp;
using Movies.Admin.Api.Data;
using Microsoft.EntityFrameworkCore;
using Movies.Admin.Api.Services.CacheServices;

namespace Movies.Admin.Api.Services
{
    public class MovieServices : IMovieServices
    {
        private readonly IConfiguration _config;
        private readonly AccountsDbContext _context;

        public MongoClient Client { get; }
        public IMongoDatabase DB { get; }

        private readonly ICacheService _cacheService;

        public MovieServices(IConfiguration config, AccountsDbContext context, ICacheService cacheService)
        {
            _config = config;
            _context = context;

            Client = new MongoClient(_config["MongoDbKey"]);
            DB = Client.GetDatabase("MoviesCluster");

            _cacheService = cacheService;
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

        public async Task<ServiceResponse<string>> SendEmail(RecommendMovieModel recommendation)
        {
            try
            {

                var user = _context.Users.Find(recommendation.UserId);
                var movie = _context.Movies.Find(recommendation.MovieId);


                var subject = $"Your Friends {user.UserName} recommeded a movie to you!!!";
                var body = $"Personal Message: {recommendation.PersonalMessage} \nTitle: {movie.Title} -- Relase Year: {movie.RelaseYear} --Image: {movie.ImageUrl} ";

                var emailToSend = new MimeMessage();
                emailToSend.From.Add(MailboxAddress.Parse(_config["EmailConfiguration:From"]));
                emailToSend.To.Add(MailboxAddress.Parse(recommendation.ToEmail));
                emailToSend.Subject = subject;
                emailToSend.Body = new TextPart(TextFormat.Plain) { Text = body };

                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 465, true);
                smtp.Authenticate(_config["EmailConfiguration:From"], _config["EmailConfiguration:Password"]);
                smtp.Send(emailToSend);
                smtp.Disconnect(true);


                //Cache EMail
                await _cacheService.SetRecommendation(recommendation);


                return new ServiceResponse<string>()
                {
                    Success =true,
                };

            }catch(Exception ex)
            {
                return new ServiceResponse<string>()
                {
                    Success =false,
                    Message = ex.Message,
                };

            }
        }
    }
}

