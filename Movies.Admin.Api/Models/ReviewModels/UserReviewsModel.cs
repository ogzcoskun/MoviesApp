using MongoDB.Bson;

namespace Movies.Admin.Api.Models.ReviewModels
{
    public class UserReviewsModel
    {
        public ObjectId ObjectId { get; set; }
        public string UserId { get; set; }
        public List<ReviewModel> Reviews { get; set; }
    }
}
