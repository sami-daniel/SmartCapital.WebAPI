namespace SmartCapital.WebAPI.Application.Exceptions
{
    public class ExistingUserException : Exception
    {
        public ExistingUserException()
        {
        }

        public ExistingUserException(string? message) : base(message)
        {
        }

        public ExistingUserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
