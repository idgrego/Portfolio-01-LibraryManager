namespace LibraryManager.API.Exceptions
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message) 
            : base(message, System.Net.HttpStatusCode.BadRequest)
        {
            
        }
    }
}
