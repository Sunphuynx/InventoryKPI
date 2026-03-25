using System;
using System.Collections.Generic;
using System.Linq;
using InventoryKPI.Models;

namespace InventoryKPI.Services
{
    public class KPIService
    {
        public KPIResult CalculateKPIs(
            Dictionary<string, InventoryState> inventory,
            List<Invoice> invoices)
        {
            var result = new KPIResult();

            // 1. Total Invoices
            result.TotalInvoices = invoices.Count;

            // 2. Total SKUs
            result.TotalSKUs = invoices
                .SelectMany(i => i.LineItems ?? new List<LineItem>())
                .Where(li => !string.IsNullOrWhiteSpace(li.AccountCode))
                .Select(li => li.AccountCode)
                .Distinct()
                .Count();

            // 3. Stock Value = Sum(Unsold Quantity x Unit Cost)
            result.StockValue = inventory.Values
                .Where(s => s.TotalPurchased > s.TotalSold)
                .Sum(s => (s.TotalPurchased - s.TotalSold) * s.UnitCost);

            // 4. Out-of-Stock Items
            result.OutOfStockItems = inventory.Values
                .Where(s => s.TotalPurchased - s.TotalSold <= 0)
                .Count();

            // 5. Average Daily Sales
            var salesInvoices = invoices
                .Where(i => i.Type == "ACCREC")
                .ToList();

            if (salesInvoices.Count > 0)
            {
                double totalSold = salesInvoices
                    .SelectMany(i => i.LineItems ?? new List<LineItem>())
                    .Sum(li => li.Quantity);

                var dates = new List<DateTime>();
                foreach (var inv in salesInvoices)
                {
                    if (DateTime.TryParse(inv.DateString, out var d))
                        dates.Add(d);
                }

                if (dates.Count > 0)
                {
                    double days = (MaxDate(dates) - MinDate(dates)).TotalDays + 1;
                    if (days > 0)
                        result.AverageDailySales = totalSold / days;
                }
            }

            // 6. Average Inventory Age = Average(Today - LastPurchaseDate)
            var today = DateTime.Now;
            var unsoldItems = new List<InventoryState>();

            foreach (var s in inventory.Values)
            {
                if (s.TotalPurchased > s.TotalSold && s.LastPurchaseDate != default(DateTime))
                    unsoldItems.Add(s);
            }

            if (unsoldItems.Count > 0)
            {
                double totalDays = 0;
                foreach (var s in unsoldItems)
                    totalDays += (today - s.LastPurchaseDate).TotalDays;

                result.AverageInventoryAge = totalDays / unsoldItems.Count;
            }

            return result;
        }

        private DateTime MaxDate(List<DateTime> dates)
        {
            var max = dates[0];
            foreach (var d in dates)
                if (d > max) max = d;
            return max;
        }

        private DateTime MinDate(List<DateTime> dates)
        {
            var min = dates[0];
            foreach (var d in dates)
                if (d < min) min = d;
            return min;
        }
    }
}
