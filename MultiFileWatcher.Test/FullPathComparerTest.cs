using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Geekality.IO
{
	[TestFixture]
    public class FullPathComparerTest
    {
        IEqualityComparer<FileSystemInfo> subject = new FullPathComparer();

        [Test]
        public void Equals_EqualPaths_ReturnsTrue()
        {
            var a = new FileInfo("test.txt");
            var b = new FileInfo("test.txt");

            Assert.True(subject.Equals(a, b));
        }

        [Test]
        public void Equals_DifferentPaths_ReturnsFalse()
        {
            var a = new FileInfo("a.txt");
            var b = new FileInfo("b.txt");

            Assert.False(subject.Equals(a, b));
        }

        [Test]
        public void GetHashCode_GivenFileInfo_ReturnsHashCodeOfFullPath()
        {
            var a = new FileInfo("a.txt");

            Assert.AreEqual(a.FullName.GetHashCode(), subject.GetHashCode(a));
        }

    }
}
