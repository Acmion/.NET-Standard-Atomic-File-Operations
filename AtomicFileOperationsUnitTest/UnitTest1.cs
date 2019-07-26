using AtomicFileOperations;
using NUnit.Framework;
using System.IO;

namespace Tests
{
    public class AtomicFileOperationTests
    {
        private static string TestDirectoryPath { get => Path.Combine(Path.GetTempPath(), "AtomicFileOperation"); }

        [SetUp]
        public void Setup()
        {
            ClearTestDirectory();
        }

        [Test]
        public void TestWriteAndReadWhenTargetDoesNotExist()
        {
            var filePath = Path.Combine(TestDirectoryPath, "test.txt");
            var contents = "Hello World!";

            AtomicFileOperation.WriteAllText(filePath, contents);

            var read = AtomicFileOperation.ReadAllText(filePath);

            Assert.AreEqual(contents, read);
        }

        private static void ClearTestDirectory()
        {
            var dirPath = TestDirectoryPath;
            if (Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath, true);
            }

            Directory.CreateDirectory(dirPath);
        }

    }
}