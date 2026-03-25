using System;
using System.Collections.Generic;
using InventoryKPI.Models;

namespace InventoryKPI.Services
{
    public class InventoryService
    {
        public Dictionary<string, InventoryState> BuildInventory(List<Invoice> invoices)
        {
            var inventory = new Dictionary<string, InventoryState>();

            foreach (var invoice in invoices)
            {
                if (!DateTime.TryParse(invoice.DateString, out var invoiceDate))
                    continue;

                var lineItems = invoice.LineItems ?? new List<LineItem>();

                foreach (var item in lineItems)
                {
                    var id = item.AccountCode;
                    if (string.IsNullOrWhiteSpace(id)) continue;

                    if (!inventory.ContainsKey(id))
                    {
                        inventory[id] = new InventoryState { ProductId = id };
                    }

                    var state = inventory[id];

                    if (invoice.Type == "ACCPAY")
                    {
                        double qty = item.Quantity;

                        if (state.Debt > 0)
                        {
                            if (qty >= state.Debt)
                            {
                                qty -= state.Debt;
                                state.Debt = 0;
                            }
                            else
                            {
                                state.Debt -= qty;
                                continue;
                            }
                        }

                        state.TotalPurchased += qty;
                        state.TotalPurchaseValue += qty * item.UnitAmount;

                        if (state.TotalPurchased > 0)
                            state.UnitCost = state.TotalPurchaseValue / state.TotalPurchased;

                        state.LastPurchaseDate = invoiceDate;
                    }

                    if (invoice.Type == "ACCREC")
                    {
                        var remaining = state.TotalPurchased - state.TotalSold;

                        if (remaining >= item.Quantity)
                        {
                            state.TotalSold += item.Quantity;
                        }
                        else
                        {
                            var deficit = item.Quantity - remaining;
                            state.TotalSold += remaining;
                            state.Debt += deficit;
                        }
                    }
                }
            }

            return inventory;
        }
    }
}
