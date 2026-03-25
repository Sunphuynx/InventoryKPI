namespace InventoryKPI.Models
{
    public class LineItem
    {
        public string Description { get; set; }
        public double UnitAmount { get; set; }
        public double Quantity { get; set; }
        public double LineAmount { get; set; }
        public string AccountCode { get; set; }
    }
}
