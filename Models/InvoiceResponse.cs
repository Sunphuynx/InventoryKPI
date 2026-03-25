using System.Collections.Generic;

namespace InventoryKPI.Models
{
    public class InvoiceResponse
    {
        public string Id { get; set; }
        public Pagination Pagination { get; set; }
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
