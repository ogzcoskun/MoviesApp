using Microsoft.AspNetCore.Mvc;
using Movies.Accounts.Api.Models.UserModels;

namespace Movies.Accounts.Api.Events.PublishEvents
{
    public interface IPublishEventsService
    {
        Task UserRegisteredEvent(UserModel user);
    }
}
