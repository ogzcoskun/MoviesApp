namespace Movies.Client.Api.Models
{
    public class ServiceResponse<T>
    {
        public ServiceResponse()
        {
        }
        public ServiceResponse(T data)
        {
            Success = true;
            Message = string.Empty;
            Data = data;
        }
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
