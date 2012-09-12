using System.IO;

namespace Geekality.MultiFileWatcher
{
    /// <summary>
    /// Data for the <see cref="FileWatcher.Changed"/> event.
    /// </summary>
    public class FileChangedEventArgs : FileEventArgs
    {
        /// <summary>
        /// Creates a new <see cref="FileChangedEventArgs"/>
        /// </summary>
        /// <param name="changedFile">The file that changed.</param>
        public FileChangedEventArgs(FileInfo changedFile) : base(changedFile) { }
    }
}
