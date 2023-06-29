namespace Movies.Admin.Api.Models.ReviewModels
{
    public class ReviewModel
    {
        public string ReviewId { get; set; }
        public string MovieId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
