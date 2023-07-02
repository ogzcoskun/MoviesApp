using DotNetCore.CAP;
using Movies.Admin.Api.Models;
using Movies.Admin.Api.Services;
using Newtonsoft.Json;

namespace Movies.Admin.Api.Events.RecieveEvents
{
    public class RecieveEvents : IRecieveEvents ,  ICapSubscribe
    {
        private readonly IMovieServices _movieService;

        public RecieveEvents(IMovieServices movieService)
        {
            _movieService = movieService;
        }

        [CapSubscribe("UserRegisteredEvent")]
        public async void UserRegisteredEventHandler(string userJson)
        {
            var user = JsonConvert.DeserializeObject<UserModel>(userJson);

            await _movieService.CreateUser(user);

        }

        [CapSubscribe("UserRecommendAMovie")]
        public async void UserRecommendedAMovie(string recommendation)
        {
            var recommendationModel = JsonConvert.DeserializeObject<RecommendMovieModel>(recommendation);

            var recommend = await _movieService.SendEmail(recommendationModel);

            //await _movieService.CreateUser(user);

        }
    }
}
