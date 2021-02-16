using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netsurit.Results
{
    /// <summary>
    /// This is a wrapper for using InnerResult. It allows for cleaner code that implements a Result object instead of null checking.
    /// </summary>
    public class ResultWrapper
    {
        /// <summary>
        /// Add new valid data to an InnerResult object
        /// </summary>
        /// <typeparam name="T">The DataType of te data</typeparam>
        /// <param name="data">The data</param>
        /// <returns></returns>
        public static Result<T> Success<T>(T data)
        where T : class
        {
            return new Result<T>(data, true);
        }

        /// <summary>
        /// Create a new InnerResult object containing an error message due to invalid data
        /// </summary>
        /// <param name="message">Error message explaining why the wished for data could not be retreived and is therefore invalid</param>
        /// <returns></returns>
        public static Result<String> Error(String message)
        {
            return new Result<String>(message, false);
        }
    }
}
