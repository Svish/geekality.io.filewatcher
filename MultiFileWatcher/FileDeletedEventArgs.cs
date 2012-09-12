using System.IO;

namespace Geekality.MultiFileWatcher
{
    /// <summary>
    /// Data for the <see cref="FileWatcher.Deleted"/> event.
    /// </summary>
    public class FileDeletedEventArgs : FileEventArgs
    {
        /// <summary>
        /// Creates a new <see cref="FileDeletedEventArgs"/>.
        /// </summary>
        /// <param name="deletedFile">The file that was deleted.</param>
        public FileDeletedEventArgs(FileInfo deletedFile) : base(deletedFile) { }
    }
}
