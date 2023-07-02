using System.ComponentModel.DataAnnotations;

namespace Movies.Client.Api.Models.Reviews
{
    public class PostReviewModel
    {
        public string MovieId { get; set; }
        [Range(0, 10)]
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
