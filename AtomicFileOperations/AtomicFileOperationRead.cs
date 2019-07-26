using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AtomicFileOperations
{
    public static partial class AtomicFileOperation
    {
        /// <summary>
        ///     Opens a text file, reads all lines of the file, and then closes the file.
        /// </summary>
        /// <param name="filePath"> 
        ///     The file to open for reading. 
        /// </param>
        /// <returns> 
        ///     A string containing all lines of the file.
        /// </returns>
        public static string ReadAllText(string filePath)
        {
            return File.ReadAllText(GetReadFilePath(filePath));
        }

        /// <summary>
        ///     Opens a file, reads all lines of the file with the specified encoding, and then
        ///     closes the file.
        /// </summary>
        /// <param name="filePath"> 
        ///     The file to open for reading. 
        /// </param>
        /// <param name="encoding">
        ///     The encoding applied to the contents of the file.
        /// </param>
        /// <returns> 
        ///     A string containing all lines of the file.
        /// </returns>
        public static string ReadAllText(string filePath, Encoding encoding)
        {
            return File.ReadAllText(GetReadFilePath(filePath), encoding);
        }

        /// <summary>
        ///     Opens a file, reads all lines of the file, and then closes the file.
        /// </summary>
        /// <param name="filePath"> 
        ///     The file to open for reading. 
        /// </param>
        /// <returns> 
        ///     A string array containing all lines of the file. 
        /// </returns>
        public static string[] ReadAllLines(string filePath)
        {
            return File.ReadAllLines(GetReadFilePath(filePath));
        }

        /// <summary>
        ///     Opens a file, reads all lines of the file with the specified encoding, and then
        ///     closes the file.
        /// </summary>
        /// <param name="filePath"> 
        ///     The file to open for reading. 
        /// </param>
        /// <param name="encoding"> 
        ///     The encoding applied to the contents of the file. 
        /// </param>
        /// <returns> 
        ///     A string array containing all lines of the file. 
        /// </returns>
        public static string[] ReadAllLines(string filePath, Encoding encoding)
        {
            return File.ReadAllLines(GetReadFilePath(filePath), encoding);
        }

        /// <summary>
        ///     Opens a binary file, reads the contents of the file into a byte array, and then
        ///     closes the file.
        /// </summary>
        /// <param name="filePath"> 
        ///     The file to open for reading. 
        /// </param>
        /// <returns> 
        ///     A byte array containing the contents of the file. 
        /// </returns>
        public static byte[] ReadAllBytes(string filePath)
        {
            return File.ReadAllBytes(GetReadFilePath(filePath));
        }

        /// <summary>
        ///     Gets the valid file path (According to AtomicFileOperation. For example, the temp file 
        ///     can be valid), if it exists.
        /// </summary>
        /// <param name="filePath">
        ///     The file to open for reading. 
        /// </param>
        /// <returns>
        ///     A string containing the valid file to read.
        /// </returns>
        private static string GetReadFilePath(string filePath)
        {
            /*
                The read process follows the steps defined in the 
                write process.

                Read process:
                If no files exist:
                    Throw error
                Else if only F exists:
                    Read F
                Else if F and T exist:
                    Read F
                Else if only T exists:
                    Read T
                Else if only S exists:
                    Throw error
                Else if S and T exist:
                    Throw error

                F = File that the operation is performed on
                S = State file
                T = Temp file
            */

            var tempFilePath = GetTempFilePath(filePath);
            var stateFilePath = GetStateFilePath(filePath);

            var filePathExists = File.Exists(filePath);
            var tempFilePathExists = File.Exists(tempFilePath);
            var stateFilePathExists = File.Exists(stateFilePath);

            if (!filePathExists && !tempFilePathExists && !stateFilePathExists)
            {
                // Error
            }
            else if (filePathExists && !tempFilePathExists && !stateFilePathExists)
            {
                return filePath;
            }
            else if (filePathExists && tempFilePathExists)
            {
                return filePath;
            }
            else if (!filePathExists && tempFilePathExists && !stateFilePathExists)
            {
                return tempFilePath;
            }
            else if (!filePathExists && !tempFilePathExists && stateFilePathExists)
            {
                // Error
            }
            else if (tempFilePathExists && stateFilePathExists)
            {
                // Error
            }

            throw new Exception("No valid file found.");
        }
    }
}
