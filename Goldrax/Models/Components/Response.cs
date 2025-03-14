namespace Goldrax.Models.Components
{
    public class Response<T>
    {
        public bool Succeeded { get; set; }

        public string? Message { get; set; }
        public T? Data { get; set; }


        public Response(bool succeeded, string message, T? data = default)
        {
            Succeeded = succeeded;
            Message = message;
            Data = data;
        }
    }
}
