using System;
using System.IO;
using System.Threading.Tasks;

namespace InventoryKPI.Services
{
    public class FileWatcherService
    {
        private readonly string invoiceFolder = Path.Combine(AppContext.BaseDirectory, "Data", "invoices");
        private readonly FileProcessorService processor;
        private FileSystemWatcher? watcher;

        public FileWatcherService(FileProcessorService processor)
        {
            this.processor = processor;
        }

        public void StartWatching()
        {
            watcher = new FileSystemWatcher(invoiceFolder, "*.json");
            watcher.Created += OnFileCreated;
            watcher.Renamed += OnFileCreated;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Watching for new invoice files...");
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Task.Run(async () =>
            {
                await WaitForFileReady(e.FullPath);
                processor.EnqueueFile(e.FullPath);
            });
        }

        private async Task WaitForFileReady(string path)
        {
            while (true)
            {
                try
                {
                    using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        if (stream.Length > 0) break;
                    }
                }
                catch { }

                await Task.Delay(100);
            }
        }
    }
}
