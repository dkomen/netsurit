using System;

namespace Netsurit.Exceptions
{
    /// <summary>
    ///  Exception used to indicate illegal access to invalid data.
    ///  Specifically maps to ApplicationException not plain Exception
    /// </summary>
    public class NoDataException : System.ApplicationException
    {
        public NoDataException(String message): base(message)
        { }
        public NoDataException(String message, Exception innerException): base(message, innerException)
        { }
    }
}
