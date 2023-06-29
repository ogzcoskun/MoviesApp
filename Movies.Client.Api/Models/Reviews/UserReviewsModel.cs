using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Movies.Client.Api.Models.Reviews
{
    public class UserReviewsModel
    {
        [BsonId]
        public ObjectId _id { get; set; }
        
        public ObjectId ObjectId { get; set; }
        public string UserId { get; set; }
        public List<ReviewModel>? Reviews { get; set; }
    }
}
