using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using InventoryKPI.Models;

namespace InventoryKPI.Services
{
    public class JsonLoaderService
    {
        public async Task<List<Invoice>> LoadInvoicesAsync(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var response = await JsonSerializer.DeserializeAsync<InvoiceResponse>(stream, options);

                return response?.Invoices ?? new List<Invoice>();
            }
        }
    }
}
