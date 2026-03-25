namespace InventoryKPI.Models
{
    public class KPIResult
    {
        public int TotalSKUs { get; set; }
        public double StockValue { get; set; }
        public int OutOfStockItems { get; set; }
        public double AverageDailySales { get; set; }
        public double AverageInventoryAge { get; set; }
        public int TotalInvoices { get; set; }
    }
}
