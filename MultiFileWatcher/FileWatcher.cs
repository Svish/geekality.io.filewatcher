using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Geekality.MultiFileWatcher
{
    /// <summary>
    /// A wrapper around <see cref="FileSystemWatcher"/> for watching multiple files located in multiple directories.
    /// 
    /// <remarks>Events are raised asynchronously using <see cref="SynchronizationContext.Post"/>.</remarks>
    /// </summary>
    public sealed class FileWatcher : IDisposable
    {
        /// <summary>
        /// Raised when one of the watched files has changed.
        /// </summary>
        public event EventHandler<FileChangedEventArgs> Changed = (s, e) => { };

        /// <summary>
        /// Raised when one of the watched files has been deleted and is no longer watched.
        /// </summary>
        public event EventHandler<FileDeletedEventArgs> Deleted = (s, e) => { };

        /// <summary>
        /// Raised when one of the watched files has been renamed and the watcher is now watching the new path instead.
        /// </summary>
        public event EventHandler<FileRenamedEventArgs> Renamed = (s, e) => { };

        private readonly FullPathComparer comparer = new FullPathComparer();
        private readonly SynchronizationContext context;

        private readonly Dictionary<FileInfo, long> watchList;
        private readonly Dictionary<DirectoryInfo, FileSystemWatcher> watchers;

        /// <summary>
        /// Creates a new <see cref="FileWatcher"/>.
        /// </summary>
        public FileWatcher() : this(null) { }

        /// <summary>
        /// Creates a new <see cref="FileWatcher"/> which will use the given <see cref="SynchronizationContext"/> to raising events.
        /// </summary>
        /// <param name="context">The <see cref="SynchronizationContext"/> to use when raising events.</param>
        public FileWatcher(SynchronizationContext context)
        {
            this.context = context ?? new SynchronizationContext();
            watchList = new Dictionary<FileInfo, long>(comparer);
            watchers = new Dictionary<DirectoryInfo, FileSystemWatcher>(comparer);
        }

        /// <summary>
        /// Adds a file to the list of files to watch.
        /// </summary>
        /// <param name="file">File to start watching.</param>
        /// <returns><c>False</c> if the file is already being watched; otherwise <c>True</c>.</returns>
        public bool Add(FileInfo file)
        {
            if (watchList.ContainsKey(file))
                return false;

            // Add to watch list
            watchList.Add(file, file.LastWriteTimeUtc.Ticks);

            // Create watcher for directory if we haven't already
            if ( ! watchers.ContainsKey(file.Directory))
            {
                var watcher = new FileSystemWatcher
                {
                    Path = file.DirectoryName,
                    EnableRaisingEvents = true,
                };
                watcher.Changed += OnFileChanged;
                watcher.Deleted += OnFileDeleted;
                watcher.Renamed += OnFileRenamed;
                watcher.Error += OnError;
                watchers.Add(file.Directory, watcher);
            }

            return true;
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            // If internal bufferoverflow
            if (sender is FileSystemWatcher && e.GetException() is InternalBufferOverflowException)
            {
                // Double internal buffer size
                var watcher = (FileSystemWatcher)sender;
                watcher.InternalBufferSize *= 2;
            }
        }

        /// <summary>
        /// Removes a file from the list of files to watch.
        /// </summary>
        /// <param name="file">File to stop watching.</param>
        public void Remove(FileInfo file)
        {
            // Remove file from watch list
            watchList.Remove(file);

            // Dispose and remove watcher for directory if no other files in same directory
            if ( ! watchList.Any(x => comparer.Equals(file.Directory, x.Key.Directory)))
            {
                watchers[file.Directory].EnableRaisingEvents = false;
                watchers[file.Directory].Dispose();
                watchers.Remove(file.Directory);
            }
        }


        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            var file = new FileInfo(e.FullPath);
            lock(watchList)
                // HACK: Checking and storing previous LastWriteTime to filter out duplicate change event
                if (watchList.ContainsKey(file) && watchList[file] < file.LastWriteTimeUtc.Ticks)
                {
                    watchList[file] = file.LastWriteTimeUtc.Ticks;
                    InvokeFileChanged(file);
                }
        }


        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            var file = new FileInfo(e.FullPath);

            if (watchList.ContainsKey(file))
            {
                Remove(new FileInfo(e.FullPath));
                InvokeFileDeleted(file);
            }
        }


        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            var newFile = new FileInfo(e.FullPath);
            var oldFile = new FileInfo(e.OldFullPath);

            // If we were watching the old file
            if (watchList.ContainsKey(oldFile))
            {
                // Remove and add new
                Remove(oldFile);
                Add(newFile);
                InvokeFileRenamed(newFile, oldFile);
            }
        }

        private void InvokeFileDeleted(FileInfo file)
        {
            context.Post(e => Deleted(this, (FileDeletedEventArgs)e), new FileDeletedEventArgs(file));
        }

        private void InvokeFileRenamed(FileInfo file, FileInfo oldFile)
        {
            context.Post(e => Renamed(this, (FileRenamedEventArgs)e), new FileRenamedEventArgs(file, oldFile));
        }

        private void InvokeFileChanged(FileInfo file)
        {
            context.Post(e => Changed(this, (FileChangedEventArgs)e), new FileChangedEventArgs(file));
        }


        /// <summary>
        /// Stops watching all files and clears the watch list.
        /// </summary>
        public void Dispose()
        {
            foreach (var x in watchList)
                Remove(x.Key);
        }
    }
}
