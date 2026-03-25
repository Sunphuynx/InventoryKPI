using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InventoryKPI.Services;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("System starting...");

var processor = new FileProcessorService();

await processor.LoadProcessedFilesData();

processor.StartProcessing();

var folder = Path.Combine(AppContext.BaseDirectory, "Data", "invoices");

foreach (var file in Directory.GetFiles(folder, "*.json"))
{
    processor.EnqueueFile(file);
}

var watcher = new FileWatcherService(processor);
watcher.StartWatching();

await Task.Delay(Timeout.Infinite);
