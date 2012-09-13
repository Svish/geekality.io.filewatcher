using System;
using System.IO;

namespace Geekality.IO
{
    class Program
    {
        static volatile bool running = true;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += OnCancelKeyPress;
            Console.WriteLine("Enter the full path of a file to start watching it. Ctrl+C to quit.");

            using (var watcher = new MultiFileWatcher())
            {
                watcher.Changed += watcher_Changed;
                watcher.Deleted += watcher_Deleted;
                watcher.Renamed += watcher_Renamed;

                while (running)
                {
                    try
                    {
                        var file = new FileInfo(Console.ReadLine());

                        if (file.Exists)
                            Console.WriteLine(watcher.Add(file) ? "Added" : "Already added");
                        else
                            Console.WriteLine("Not a file");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        static void watcher_Renamed(object sender, FileRenamedEventArgs e)
        {
            Console.WriteLine("File renamed: {1} -> {0}", e.File, e.OldFile);
        }

        static void watcher_Deleted(object sender, FileDeletedEventArgs e)
        {
            Console.WriteLine("File deleted: {0}", e.File);
        }

        static void watcher_Changed(object sender, FileChangedEventArgs e)
        {
            Console.WriteLine("File changed: {0}", e.File);
        }

        static void OnCancelKeyPress(object sender, System.ConsoleCancelEventArgs e)
        {
            running = false;
            Console.WriteLine("Quit");
        }
    }
}
