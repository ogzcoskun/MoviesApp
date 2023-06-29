namespace Movies.Client.Api.Models
{
    public class RecommendMovieModel
    {
        public string MovieId { get; set; }
        public string UserId { get; set; }
        public string ToEmail { get; set; }
        public string PersonalMessage { get; set; }
    }
}
