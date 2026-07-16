namespace TAO.SharedKernel.Exceptions
{
    public abstract class TaoException : Exception
    {
        protected TaoException(string message)
            : base(message)
        {
        }

        protected TaoException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
