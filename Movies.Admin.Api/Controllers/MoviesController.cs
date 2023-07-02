using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Movies.Admin.Api.Models;
using Movies.Admin.Api.Models.ReviewModels;
using Movies.Admin.Api.Services;
using Movies.Admin.Api.Services.CacheServices;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Movies.Admin.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieServices _moviesServices;
        private readonly ICacheService _cacheService;

        public MoviesController(IMovieServices moviesServices, ICacheService cacheService)
        {
            _moviesServices = moviesServices;
            _cacheService = cacheService;
        }

        //[HttpPost]
        //public async Task<IActionResult> TestRedis(RecommendMovieModel recomm)
        //{

        //    //var response = await _moviesServices.SendEmail(recomm);
        //    var response = await _cacheService.SetRecommendation(recomm);


        //    return Ok(response);
        //}

        //[HttpGet]
        //public async Task<IActionResult> TestRedis2(string userId)
        //{

        //    //var response = await _moviesServices.SendEmail(recomm);
        //    var response = await _cacheService.GetRecommendations(userId);


        //    return Ok(response);
        //}



    }
}
