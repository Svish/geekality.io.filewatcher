using System.IO;

namespace Geekality.IO
{
    /// <summary>
    /// Data for the <see cref="MultiFileWatcher.Renamed"/> event.
    /// </summary>
    public class FileRenamedEventArgs : FileEventArgs
    {
        /// <summary>
        /// The old file which was renamed and no longer exists.
        /// </summary>
        public FileInfo OldFile { get; private set; }

        /// <summary>
        /// Creates a new <see cref="FileRenamedEventArgs"/>.
        /// </summary>
        /// <param name="newFile">The new file which has been renamed and now exists.</param>
        /// <param name="oldFile">The old file which was renamed and no longer exists.</param>
        public FileRenamedEventArgs(FileInfo newFile, FileInfo oldFile) : base(newFile)
        {
            OldFile = oldFile;
        }

    }
}
