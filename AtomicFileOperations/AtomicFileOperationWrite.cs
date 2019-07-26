using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AtomicFileOperations
{
    public static partial class AtomicFileOperation
    {
        /// <summary>
        ///     Creates a new file, writes the specified byte array to the file, and then closes
        ///     the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="filePath">
        ///     The file to write to.
        /// </param>
        /// <param name="bytes">
        ///     The bytes to write to the file.
        /// </param>
        public static void WriteAllBytes(string filePath, byte[] bytes)
        {
            WriteOperation(filePath, bytes);
        }

        /// <summary>
        ///     Creates a new file, writes the specified string to the file, and then closes
        ///     the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="filePath">
        ///     The file to write to.
        /// </param>
        /// <param name="contents">
        ///     The string to write to the file.
        /// </param>
        public static void WriteAllText(string filePath, string contents)
        {
            WriteAllBytes(filePath, new UTF8Encoding(true).GetBytes(contents));
        }

        /// <summary>
        ///     Creates a new file, writes the specified string to the file using the specified
        ///     encoding, and then closes the file. If the target file already exists, it is
        ///     overwritten.
        /// </summary>
        /// <param name="filePath">
        ///     The file to write to.
        /// </param>
        /// <param name="contents">
        ///     The string to write to the file.
        /// </param>
        /// <param name="encoding">
        ///     The encoding to apply to the string.
        /// </param>
        public static void WriteAllText(string filePath, string contents, Encoding encoding)
        {
            WriteAllBytes(filePath, encoding.GetBytes(contents));
        }

        /// <summary>
        ///     Creates a new file, writes a collection of strings to the file, and then closes
        ///     the file.
        /// </summary>
        /// <param name="filePath">
        ///     The file to write to.
        /// </param>
        /// <param name="lines">
        ///     The lines to write to the file.
        /// </param>
        public static void WriteAllLines(string filePath, IEnumerable<string> lines)
        {
            WriteAllText(filePath, string.Join(Environment.NewLine, lines));
        }

        /// <summary>
        ///     Creates a new file by using the specified encoding, writes a collection of strings
        ///     to the file, and then closes the file.
        /// </summary>
        /// <param name="filePath">
        ///     The file to write to.
        /// </param>
        /// <param name="lines">
        ///     The lines to write to the file.
        /// </param>
        /// <param name="encoding">
        ///     The character encoding to use.
        /// </param>
        public static void WriteAllLines(string filePath, IEnumerable<string> lines, Encoding encoding)
        {
            WriteAllText(filePath, string.Join(Environment.NewLine, lines), encoding);
        }

        /// <summary>
        ///     Writes the bytes to a file atomically.
        /// </summary>
        /// <param name="filePath">
        ///     The file to write to.
        /// </param>
        /// <param name="bytesToWrite">
        ///     The bytes to write to the file.
        /// </param>
        private static void WriteOperation(string filePath, byte[] bytesToWrite)
        {
            VerifyFilePath(filePath);
            Clean(filePath);

            if (File.Exists(filePath))
            {
                WriteOperationTargetExists(filePath, bytesToWrite);
            }
            else
            {
                WriteOperationTargetNotExists(filePath, bytesToWrite);
            }
        }

        /// <summary>
        ///     Writes the bytes to an existing file atomically.
        /// </summary>
        /// <param name="filePath">
        ///     The file to write to.
        /// </param>
        /// <param name="bytesToWrite">
        ///     The bytes to write to the file.
        /// </param>
        private static void WriteOperationTargetExists(string filePath, byte[] bytesToWrite)
        {
            /*
                Write operation steps when target file exists at start:
                E0: File system original state.
                E1: Write and flush new data into the temp file.
                E2: Delete target file.
                E3: Rename temp file to target file.

                Existing files after completed steps when target file exists at start:
                E0: *F*
                E1: *F* T
                E2:    *T*
                E3: *F*

                F = File that the operation is performed on
                S = State file
                T = Temp file
            
                *F* = Target file is valid.
                *T* = Temp file is valid.
                * * = Target file does not exist, but this state is valid.
            */

            var tempFilePath = GetTempFilePath(filePath);

            WriteTempFile(tempFilePath, bytesToWrite);

            File.Delete(filePath);
            File.Move(tempFilePath, filePath);
        }

        /// <summary>
        ///     Writes the bytes to an non existing file atomically.
        /// </summary>
        /// <param name="filePath">
        ///     The file to write to.
        /// </param>
        /// <param name="bytesToWrite">
        ///     The bytes to write to the file.
        /// </param>
        private static void WriteOperationTargetNotExists(string filePath, byte[] bytesToWrite)
        {
            /*
                Write operation steps when target file does not exist at start:
                N0: File system original state.
                N1: Create a state file that denotes that no target file existed at start.
                N2: Write and flush new data into the temp file.
                N3: Delete state file.
                N4: Rename temp file to target file.    

                Existing files after completed steps when target file does not exist at start:
                N0: * *
                N1: * * S
                N2: * * S  T
                N3:       *T*
                N4: *F*

                F = File that the operation is performed on
                S = State file
                T = Temp file
            
                *F* = Target file is valid.
                *T* = Temp file is valid.
                * * = Target file does not exist, but this state is valid.
            */

            var tempFilePath = GetTempFilePath(filePath);
            var stateFilePath = GetStateFilePath(filePath);

            using (var fs = new FileStream(stateFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Flush(true);
            }

            WriteTempFile(tempFilePath, bytesToWrite);

            File.Delete(stateFilePath);
            File.Move(tempFilePath, filePath);
        }

        /// <summary>
        ///     Writes and flushes the bytes to a temp file.
        /// </summary>
        /// <param name="tempFilePath">
        ///     The temp file to write to.
        /// </param>
        /// <param name="bytesToWrite">
        ///     The bytes to write to the temp file.
        /// </param>
        private static void WriteTempFile(string tempFilePath, byte[] bytesToWrite)
        {
            using (var fs = new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.Write(bytesToWrite, 0, bytesToWrite.Length);

                fs.Flush(true);
            }
        }
    }
}
