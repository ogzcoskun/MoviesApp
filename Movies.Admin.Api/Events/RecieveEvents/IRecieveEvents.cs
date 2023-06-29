namespace Movies.Admin.Api.Events.RecieveEvents
{
    public interface IRecieveEvents
    {
        public void UserRegisteredEventHandler(string userJson);
    }
}
