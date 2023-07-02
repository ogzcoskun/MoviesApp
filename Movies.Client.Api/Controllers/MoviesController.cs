using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Client.Api.Config;
using Movies.Client.Api.Models;
using Movies.Client.Api.Models.Reviews;
using Movies.Client.Api.Services;
using System.Security.Claims;

namespace Movies.Client.Api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesService _service;

        public MoviesController(IMoviesService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMoviesPaged([FromQuery] PaginationFilter filter)
        {
            try
            {

                var response = await _service.GetAllMovies(filter);

                return Ok(response);

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostComment(PostReviewModel postReview)
        {

            var review = new ReviewModel()
            {
                MovieId = postReview.MovieId,
                Rating = postReview.Rating,
                Comment = postReview.Comment,
            };

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId == null)
            {
                return Unauthorized();
            }

            try
            {

                var response = await _service.AddComment(userId, review);

                if (!response.Success)
                {
                    return BadRequest(response);
                }


                return Ok(response);

            }
            catch(Exception ex)
            {
                return BadRequest(new ServiceResponse<ReviewModel>()
                {
                    Success = false,
                    Message = ex.Message
                });
            } 
        }

        [HttpGet]
        public async Task<IActionResult> GetMovieWithId(string movieId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                var response = await _service.GetMovieWithId(userId, movieId);

                if (!response.Success)
                {
                    return BadRequest(response);
                }

                return Ok(response);


            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RecommendAMovie([FromQuery] string toEmail, string movieId, string personalMessage)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {
                var response = await _service.RecommendMovie(userId, toEmail, movieId, personalMessage);

                if (!response.Success)
                {
                    return BadRequest(response);
                }

                return Ok(response);



            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyRecommendations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            try
            {

                var response = await _service.GetRecommendations(userId);

                return Ok(response);


            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}
