using System.Collections.Generic;

namespace InventoryKPI.Models
{
    public class Invoice
    {
        public string InvoiceID { get; set; }
        public string InvoiceNumber { get; set; }
        public string Type { get; set; }
        public decimal Total { get; set; }
        public string DateString { get; set; }
        public List<LineItem> LineItems { get; set; } = new List<LineItem>();
    }
}
