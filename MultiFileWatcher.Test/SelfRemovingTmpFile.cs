using System;
using System.IO;
using NUnit.Framework;

namespace Geekality.IO
{
    public sealed class SelfRemovingTmpFile : IDisposable
    {
        private readonly FileInfo fileInfo;
        public SelfRemovingTmpFile()
        {
            fileInfo = new FileInfo(Path.GetTempFileName());
        }

        public static implicit operator FileInfo(SelfRemovingTmpFile file)
        {
            return file.fileInfo;
        }

        public void Dispose()
        {
            fileInfo.Delete();
        }

        public string FullName { get { return fileInfo.FullName; } }

        public bool Exists { get { fileInfo.Refresh(); return fileInfo.Exists; } }

        ~SelfRemovingTmpFile()
        {
            Dispose();
        }
    }

    [TestFixture]
    public class SelfRemovingTmpFileTest
    {
        [Test]
        public void ConstructorAndDispose_FileIsCreatedAndDeleted()
        {
            var file = new SelfRemovingTmpFile();
            using (file)
            {
                Assert.True(file.Exists);
            }
            Assert.False(file.Exists);
        }

        [Test]
        public void Finalizer_FileIsDeleted()
        {
            var file = new SelfRemovingTmpFile();
            FileInfo fileInfo = file;
            Assume.That(fileInfo.Exists, "Temporary should exist at this point.");

            file = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            fileInfo.Refresh();
            Assert.False(fileInfo.Exists);
        }
    }
}
