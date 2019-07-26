using AtomicFileOperations;
using System;
using System.IO;

namespace AtomicFileOperationsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"D:\Jnk\asd\t.txt";

            var res = AtomicFileOperation.ReadAllText(path);
        }
    }
}
