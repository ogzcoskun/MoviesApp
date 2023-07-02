using DbFillerApp.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbFillerApp
{
    public class MovieServices
    {

        


        public List<MovieModel> GetRandomMovies()
        {

            var client = new RestClient("https://moviesdatabase.p.rapidapi.com/titles/random?list=most_pop_movies");
            var restRequest = new RestRequest();
            
            restRequest.AddHeader("X-RapidAPI-Key", "45d846d93fmsh5a85413e4cee864p1b3b35jsncefffbb82adf");

            var response = client.ExecuteGet(restRequest);

            var movies = JsonConvert.DeserializeObject<MovieApiResponseModel>(response.Content);

            var moviesList = new List<MovieModel>();

            foreach (var movie in movies.results)
            {

                var id = movie.id == null ? "" : movie.id;
                var title = movie.titleText.text == null ? "" : movie.titleText.text;
                var relaseYear = movie.releaseYear == null ? 0 : movie.releaseYear.year;
                var caption = movie.primaryImage == null ? "" : movie.primaryImage.caption.plainText;
                var url = movie.primaryImage == null ? "" : movie.primaryImage.url;

                var movieToAdd = new MovieModel()
                {
                    Id = id,
                    Title = title,
                    RelaseYear = relaseYear.ToString(),
                    Caption = caption,
                    ImageUrl = url
                };

                moviesList.Add(movieToAdd);


            }


            return moviesList;

        }

        public void SaveMoviesIntoDb()
        {
            var movies = GetRandomMovies();

            var DbOptions = new DbContextOptionsBuilder<MoviesDbContext>()
            .UseSqlServer("Server= localhost;Database=MoviesDb2;Trusted_Connection=True;MultipleActiveResultSets=true")
            .Options;
            using (var DbContext = new MoviesDbContext(DbOptions))
            {

                foreach (var movie in movies)
                {
                    var check = DbContext.Movies.FirstOrDefault(x => x.Id == movie.Id);

                    if (check == null)
                    {
                        try
                        {
                            movie.Rating = 0;
                            movie.RatingCount = 0;

                            DbContext.Movies.Add(movie);
                            DbContext.SaveChanges();
                        }
                        catch(Exception ex)
                        {

                        }
         
                    }

                }

            }



        }

    }
}
