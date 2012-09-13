using System.IO;
using System.Threading;
using NUnit.Framework;

namespace Geekality.IO
{
    [TestFixture]
    public class FileWatcherTest
    {
        private MultiFileWatcher subject;
        private SelfRemovingTmpFile tmpFile;

        [SetUp]
        public void setup()
        {
            subject = new MultiFileWatcher();
            tmpFile = new SelfRemovingTmpFile();
        }

        [TearDown]
        public void tearDown()
        {
            tmpFile.Dispose();
        }


        [Test]
        public void Add_UnwatchedFile_ReturnsTrue()
        {
            Assert.True(subject.Add(tmpFile));
        }

        [Test]
        public void Add_WatchedFile_ReturnsFalse()
        {
            subject.Add(tmpFile);
            Assert.False(subject.Add(tmpFile));
        }


        [Test]
        public void Changed_FileChanges_EventIsRaisedForCorrectFile()
        {
            subject.Add(new SelfRemovingTmpFile());
            subject.Add(tmpFile);
            subject.Add(new SelfRemovingTmpFile());

            FileInfo eventResult = null;
            var eventRaised = new ManualResetEvent(false);
            subject.Changed += (s, e) => { eventResult = e.File; eventRaised.Set(); };

            File.AppendAllText(tmpFile.FullName, "test");

            Assert.True(eventRaised.WaitOne(1500), "Event was not raised.");
            Assert.AreEqual(tmpFile.FullName, eventResult.FullName);
        }

        [Test]
        public void Changed_UnwatchedFileChanges_EventIsNotRaised()
        {
            using (var otherTmpFile = new SelfRemovingTmpFile())
            {
                subject.Add(tmpFile);

                var eventRaised = new ManualResetEvent(false);
                subject.Changed += (s, e) => { eventRaised.Set(); };

                File.AppendAllText(otherTmpFile.FullName, "test");

                Assert.False(eventRaised.WaitOne(1500), "Event was raised.");
            }
        }


        [Test]
        public void Remove_OneOfMultipleFilesInSameDirectory_OtherFilesAreStillWatched()
        {
            using (var otherTmpFile = new SelfRemovingTmpFile())
            {
                subject.Add(tmpFile);
                subject.Add(otherTmpFile);

                subject.Remove(otherTmpFile);

                var eventRaised = new ManualResetEvent(false);
                subject.Changed += (s, e) => { eventRaised.Set(); };

                File.AppendAllText(tmpFile.FullName, "test");

                Assert.True(eventRaised.WaitOne(1500), "Event was not raised for file which should still be watched.");
            }
        }


        [Test]
        public void Renamed_FileIsRenamed_EventIsRaisedWithCorrectNames()
        {
            subject.Add(tmpFile);

            var eventRaised = new ManualResetEvent(false);
            FileInfo oldFile = null;
            FileInfo newFile = null;
            subject.Renamed += (s, e) => { oldFile = e.OldFile; newFile = e.File; eventRaised.Set(); };

            string newFileName = Path.GetTempPath() + Path.GetRandomFileName();
            File.Move(tmpFile.FullName, newFileName);

            Assert.True(eventRaised.WaitOne(1500), "Event was not raised.");
            Assert.True(newFile.Exists);
            Assert.False(oldFile.Exists);

            newFile.Delete();
        }

        [Test]
        public void Deleted_FileIsDeleted_EventIsRaised()
        {
            subject.Add(tmpFile);

            FileInfo eventResult = null;
            var eventRaised = new AutoResetEvent(false);
            subject.Deleted += (s, e) => { eventResult = e.File; eventRaised.Set(); };

            tmpFile.Dispose();

            Assert.True(eventRaised.WaitOne(1500), "Event was not raised.");
            Assert.AreEqual(tmpFile.FullName, eventResult.FullName);
        }
    }
}
