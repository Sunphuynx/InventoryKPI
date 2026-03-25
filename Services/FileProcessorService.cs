using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InventoryKPI.Models;

namespace InventoryKPI.Services
{
    public class FileProcessorService
    {
        private readonly JsonLoaderService loader = new JsonLoaderService();
        private readonly InventoryService inventoryService = new InventoryService();
        private readonly KPIService kpiService = new KPIService();

        private readonly BlockingCollection<string> queue = new BlockingCollection<string>();
        private readonly HashSet<string> processedFiles = new HashSet<string>();
        private readonly object lockObj = new object();
        private readonly List<Invoice> invoices = new List<Invoice>();

        private const string processedFilePath = "processed_files.txt";

        public FileProcessorService()
        {
            if (File.Exists(processedFilePath))
            {
                foreach (var line in File.ReadAllLines(processedFilePath))
                {
                    processedFiles.Add(line);
                }
            }
        }

        public void EnqueueFile(string path)
        {
            var name = Path.GetFileName(path);

            lock (lockObj)
            {
                if (processedFiles.Contains(name))
                {
                    Console.WriteLine($"Skip file da xu ly: {name}");
                    return;
                }
            }

            queue.Add(path);
        }

        public void StartProcessing()
        {
            Task.Run(async () =>
            {
                foreach (var file in queue.GetConsumingEnumerable())
                {
                    try
                    {
                        var fileName = Path.GetFileName(file);
                        Console.WriteLine($"\nDang xu ly file: {fileName}");

                        var data = await loader.LoadInvoicesAsync(file);
                        Console.WriteLine($"So invoice doc duoc: {data.Count}");

                        lock (invoices)
                        {
                            invoices.AddRange(data);
                        }

                        MarkProcessed(file);
                        UpdateKPI(fileName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Loi: {ex.Message}");
                    }
                }
            });
        }

        private void MarkProcessed(string path)
        {
            var name = Path.GetFileName(path);

            lock (lockObj)
            {
                if (!processedFiles.Contains(name))
                {
                    processedFiles.Add(name);
                    File.AppendAllText(processedFilePath, name + Environment.NewLine);
                }
            }
        }

        public async Task LoadProcessedFilesData()
        {
            var folder = Path.Combine(AppContext.BaseDirectory, "Data", "invoices");
            Console.WriteLine("Khoi phuc du lieu cu...");

            foreach (var file in Directory.GetFiles(folder, "*.json"))
            {
                var name = Path.GetFileName(file);

                if (processedFiles.Contains(name))
                {
                    var data = await loader.LoadInvoicesAsync(file);

                    lock (invoices)
                    {
                        invoices.AddRange(data);
                    }
                }
            }

            Console.WriteLine("Khoi phuc xong du lieu");
            UpdateKPI("RESTART");
        }

        private void UpdateKPI(string source = "")
        {
            List<Invoice> snapshot;

            lock (invoices)
            {
                snapshot = new List<Invoice>(invoices);
            }

            var inventory = inventoryService.BuildInventory(snapshot);
            var kpi = kpiService.CalculateKPIs(inventory, snapshot);

            Console.WriteLine("\n====================================");
            if (!string.IsNullOrEmpty(source))
                Console.WriteLine($"KPI sau khi xu ly: {source}");

            Console.WriteLine($"Tong invoices:  {kpi.TotalInvoices}");
            Console.WriteLine($"SKU:            {kpi.TotalSKUs}");
            Console.WriteLine($"Stock Value:    {kpi.StockValue:F2}");
            Console.WriteLine($"Out-of-stock:   {kpi.OutOfStockItems}");
            Console.WriteLine($"Avg Sales/day:  {kpi.AverageDailySales:F2}");
            Console.WriteLine($"Inventory Age:  {kpi.AverageInventoryAge:F2} ngay");
            Console.WriteLine("====================================\n");
        }
    }
}
