namespace SmartCapital.WebAPI.Application.Exceptions
{
    public class ExistingProfileException : Exception
    {
        public ExistingProfileException()
        {
        }

        public ExistingProfileException(string? message) : base(message)
        {
        }

        public ExistingProfileException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
