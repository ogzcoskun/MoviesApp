namespace Movies.Client.Api.Models.Reviews
{
    public class ReviewModel
    {
        public string ReviewId { get; set; }
        public string MovieId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
