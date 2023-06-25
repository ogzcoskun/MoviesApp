using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Movies.Accounts.Api.Data;
using Movies.Accounts.Api.Models.UserModels;
using Newtonsoft.Json;

namespace Movies.Accounts.Api.Events.PublishEvents
{
    public class PublishEventsService : IPublishEventsService
    {
        private readonly ICapPublisher _capPublisher;

        public PublishEventsService(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task UserRegisteredEvent(UserModel user)
        {
            try
            {
                var userJson = JsonConvert.SerializeObject(user);

                _capPublisher.Publish("UserRegisteredEvent", userJson);
            }
            catch(Exception ex)
            {

            }


        }


    }
}
