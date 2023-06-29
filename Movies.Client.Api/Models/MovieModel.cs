using System.ComponentModel.DataAnnotations;

namespace Movies.Client.Api.Models
{
    public class MovieModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string RelaseYear { get; set; }
        public string Caption { get; set; }
        public string ImageUrl { get; set; }

        [Range(0, 10)]
        public int Rating { get; set; }
        public int RatingCount { get; set; }
    }
}
