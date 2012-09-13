using System.IO;

namespace Geekality.IO
{
    /// <summary>
    /// Data for the <see cref="MultiFileWatcher.Changed"/> event.
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
