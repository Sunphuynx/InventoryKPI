using System;

namespace InventoryKPI.Models
{
    public class InventoryState
    {
        public string ProductId { get; set; }
        public double TotalPurchased { get; set; }
        public double TotalSold { get; set; }
        public double TotalPurchaseValue { get; set; }
        public double UnitCost { get; set; }
        public double Debt { get; set; }
        public DateTime LastPurchaseDate { get; set; }
    }
}
