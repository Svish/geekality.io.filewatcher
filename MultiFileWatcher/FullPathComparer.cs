using System.Collections.Generic;
using System.IO;

namespace Geekality.IO
{
    /// <summary>
    /// Compares two <see cref="FileSystemInfo"/>s based on their <see cref="FileSystemInfo.FullPath"/> property.
    /// </summary>
    public class FullPathComparer : IEqualityComparer<FileSystemInfo>
    {
        private IEqualityComparer<string> comparer = EqualityComparer<string>.Default;

        /// <returns><c>True if the <see cref="FileSystemInfo.FullName"/>s are equal; otherwise false.</c></returns>
        public bool Equals(FileSystemInfo x, FileSystemInfo y)
        {
            return comparer.Equals(x.FullName, y.FullName);
        }

        /// <returns>The hash code of the <see cref="FileSystemInfo.FullName"/>.</returns>
        public int GetHashCode(FileSystemInfo obj)
        {
            return comparer.GetHashCode(obj.FullName);
        }
    }
}
