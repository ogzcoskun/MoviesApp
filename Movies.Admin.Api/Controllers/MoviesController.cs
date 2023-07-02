using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Movies.Admin.Api.Models;
using Movies.Admin.Api.Models.ReviewModels;
using Movies.Admin.Api.Services;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Movies.Admin.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieServices _moviesServices;

        public MoviesController(IMovieServices moviesServices)
        {
            _moviesServices = moviesServices;
        }

        //[HttpPost]
        //public async Task<IActionResult> TestEmail(UserModel user)
        //{

        //    var response = await _moviesServices.SendEmail(user);


        //    return Ok(response);
        //}

    }
}
