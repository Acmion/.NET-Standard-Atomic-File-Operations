using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AtomicFileOperations
{
    /// <summary>
    /// Atomic file operations. 
    /// </summary>
    public static partial class AtomicFileOperation
    {
        /*
            When dealing with writing data to a disk several problems exist.
            The worst problems are caused by random power outages or random crashes, 
            which may corrupt a file completely. Thus, it would be preferable to have
            a procedure where the state of the system is always valid. In other words,
            a file operation must either be completely finished or no changes should be
            made at all. This is called atomicity, but is not completely achievable with
            a standard file system. Thus, we settle for a procedure where the data can
            always be recalled to a valid state, which mimicks atomicity.

            All specified operations in this document can fail during or after the execution
            call, but a valid file state should always be recallable. These steps do not explicitly
            note that the state of the file system is always cleaned before a write process.

            
        */

        private static string TempFileExtension { get; } = ".tmp";
        private static string StateFileExtension { get; } = ".stt";

        


        private static void VerifyFilePath(string filePath)
        {
            if (Directory.Exists(filePath))
            {
                throw new Exception(String.Format("The path '{0}' is a directory, not a file. Can not write contents.", filePath));
            }
        }

        private static string GetStateFilePath(string filePath)
        {
            return filePath + StateFileExtension;
        }

        private static string GetTempFilePath(string filePath)
        {
            return filePath + TempFileExtension;
        }

    }

}
