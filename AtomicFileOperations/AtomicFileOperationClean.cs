using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AtomicFileOperations
{
    public static partial class AtomicFileOperation
    {
        /// <summary>
        ///     Cleans the leftover files used by AtomicFileOperation. At the
        ///     end no files exist, or only the file defined by filePath exist.
        /// </summary>
        /// <param name="filePath">
        ///     The file that should be cleaned.
        /// </param>
        public static void Clean(string filePath)
        {
            /*
                Clean process:
                If no files exist:
                    State OK
                    End clean
                Else if only F exists:
                    State OK
                    End clean
                Else if F and T exist:
                    Delete T
                    Continue cleaning
                Else if only T exists:
                    Rename T to F
                    Continue cleaning
                Else if only S exists:
                    Delete S
                    Continue cleaning
                Else if S and T exist:
                    Delete T
                    Continue cleaning
            */

            var tempFilePath = GetTempFilePath(filePath);
            var stateFilePath = GetStateFilePath(filePath);

            var filePathExists = File.Exists(filePath);
            var tempFilePathExists = File.Exists(tempFilePath);
            var stateFilePathExists = File.Exists(stateFilePath);

            if (!filePathExists && !tempFilePathExists && !stateFilePathExists)
            {
                return;
            }
            else if (filePathExists && !tempFilePathExists && !stateFilePathExists)
            {
                return;
            }
            else if (filePathExists && tempFilePathExists)
            {
                File.Delete(tempFilePath);
                Clean(filePath);
            }
            else if (!filePathExists && tempFilePathExists && !stateFilePathExists)
            {
                File.Move(tempFilePath, filePath);
                Clean(filePath);
            }
            else if (!filePathExists && !tempFilePathExists && stateFilePathExists)
            {
                File.Delete(stateFilePath);
                Clean(filePath);
            }
            else if (tempFilePathExists && stateFilePathExists)
            {
                File.Delete(tempFilePath);
                Clean(filePath);
            }

        }

    }
}
