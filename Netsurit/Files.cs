using System;
using Netsurit.Results;


namespace Netsurit
{
    /// <summary>
    /// General disk file handling methods
    /// </summary>
    public class Files
    {
        /// <summary>
        /// Retrieve a files' contents as a string
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Result<String> ReadFileToString(String path)
        {
            if (System.IO.File.Exists(path)) //Check file exists so that we don't throw an expensive exception and also therefore do not code according to exceptions
            {
                String fileData;
                try
                {
                    fileData = System.IO.File.ReadAllText(path);
                }
                catch //Should be a rare situation - but maybe group policies block us etc?
                {
                    //We can choose to log the exception here (...Logger not included, I know)
                    return ResultWrapper.Error("Could not read the file: " + path); 
                }

                //All good so return the data
                return ResultWrapper.Success(fileData);
            } else
            {
                return ResultWrapper.Error("File path does not exist: " + path);
            }
        }

        /// <summary>
        /// Save a string to a file, delete it fisrt if able
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data">Data to write to file</param>
        /// <returns></returns>
        public static Result<String> SaveStringToFile(String path, String data)
        {

            Result<String> deleteResult = DeleteFile(path);
            if (deleteResult.IsOk)
            {
                try
                {
                    System.IO.File.WriteAllText(path, data);
                }
                catch
                {
                    return ResultWrapper.Error("Could not write to file: " + path);
                }

                return ResultWrapper.Success("Wrote to file");
            } 
            else
            {
                return deleteResult;
            }
        }

        /// <summary>
        /// Try to delete file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Result<String> DeleteFile(String path)
        {
            if (System.IO.File.Exists(path))
            {
                try
                {
                    System.IO.File.Delete(path);
                }
                catch 
                {
                    return ResultWrapper.Error("Could not delete the file: " + path);
                }
            }

            return ResultWrapper.Success("File deleted");
        }
    }
}
