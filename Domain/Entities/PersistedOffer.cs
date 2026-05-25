using System.Collections.Generic;
using System.Text.Json;
using TMPP_CRM.Domain.Common;

namespace TMPP_CRM.Domain.Entities
{
    /// <summary>Builder pattern — ofertă comercială salvată în DB</summary>
    public class PersistedOffer : BaseEntity
    {
        public string Title       { get; set; } = string.Empty;
        public string ClientName  { get; set; } = string.Empty;
        public string OfferType   { get; set; } = "standard";
        public decimal Discount   { get; set; }
        public int ValidityDays   { get; set; }
        public string Notes       { get; set; } = string.Empty;
        public string ProductsJson { get; set; } = "[]";

        public List<string> GetProducts() =>
            JsonSerializer.Deserialize<List<string>>(ProductsJson) ?? new();

        public void SetProducts(IEnumerable<string> products) =>
            ProductsJson = JsonSerializer.Serialize(products);
    }
}
