using System;
using System.IO;

namespace Geekality.IO
{
    /// <summary>
    /// Base event data for <see cref="MultiFileWatcher"/> events.
    /// </summary>
    public abstract class FileEventArgs : EventArgs
    {
        /// <summary>
        /// The affected file.
        /// </summary>
        public FileInfo File { get; private set; }

        /// <summary>
        /// Creates a new <see cref="FileEventArgs"/>.
        /// </summary>
        /// <param name="file">The affected file.</param>
        public FileEventArgs(FileInfo file)
        {
            File = file;
        }
    }
}
