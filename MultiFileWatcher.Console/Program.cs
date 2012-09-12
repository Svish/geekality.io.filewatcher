using System;
using System.IO;
using C = System.Console;

namespace Geekality.MultiFileWatcher.Console
{
    class Program
    {
        static volatile bool running = true;

        static void Main(string[] args)
        {
            C.CancelKeyPress += OnCancelKeyPress;
            C.WriteLine("Enter the full path of a file to start watching it. Ctrl+C to quit.");

            using (var watcher = new FileWatcher())
            {
                watcher.Changed += watcher_Changed;
                watcher.Deleted += watcher_Deleted;
                watcher.Renamed += watcher_Renamed;

                while (running)
                {
                    try
                    {
                        var file = new FileInfo(C.ReadLine());

                        if (file.Exists)
                            C.WriteLine(watcher.Add(file) ? "Added" : "Already added");
                        else
                            C.WriteLine("Not a file");
                    }
                    catch (Exception e)
                    {
                        C.WriteLine(e.Message);
                    }
                }
            }
        }

        static void watcher_Renamed(object sender, FileRenamedEventArgs e)
        {
            C.WriteLine("File renamed: {1} -> {0}", e.File, e.OldFile);
        }

        static void watcher_Deleted(object sender, FileDeletedEventArgs e)
        {
            C.WriteLine("File deleted: {0}", e.File);
        }

        static void watcher_Changed(object sender, FileChangedEventArgs e)
        {
            C.WriteLine("File changed: {0}", e.File);
        }

        static void OnCancelKeyPress(object sender, System.ConsoleCancelEventArgs e)
        {
            running = false;
            C.WriteLine("Quit");
        }
    }
}
