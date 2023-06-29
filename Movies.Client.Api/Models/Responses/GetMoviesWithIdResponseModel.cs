namespace Movies.Client.Api.Models.Responses
{
    public class GetMoviesWithIdResponseModel
    {
        public MovieModel Movie { get; set; }
        public int? UserRating { get; set; }
        public string UserComment { get; set; }
    }
}
