using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netsurit.Results
{
    /// <summary>
    /// Class to indicate if we have object data of type T which is NOT null.
    /// This class is used so that we minimise the usage of null comparitors in our code.
    /// It is larely unused in this test app... but i just indicate here an intent.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
        where T : class //notnull constraint requires C#8+, class should be fine
    {
        private readonly T _data = default(T); //I prefer using '_' for fields. I can explain why I prefer to not use 'this.' on field variables;
        /// <summary>
        /// Do we have a valid result?
        /// </summary>
        public bool IsOk { get; private set; }

        /// <summary>
        /// If we have no data than here you can get the reason for it.
        /// </summary>
        public String ErroMessage { get; private set; }

        public Result(T data, bool isDataOk)
        {
            this.IsOk = isDataOk;
            if (isDataOk)
            {
                _data = data;
            } else
            {
                String errorMessage = data as String;
                if (errorMessage != null)
                {
                    ErroMessage = data as String;
                } else
                {
                    ErroMessage = "Error setting error message: data = null";
                }
            }
        }

        /// <summary>
        /// Extract the data object.
        /// </summary>
        /// <returns></returns>
        public T Unwrap()
        {
            if(IsOk)
            {
                return _data;
            } else
            {
                throw new Exceptions.NoDataException("Object has no data");
            }
        }
    }
}
