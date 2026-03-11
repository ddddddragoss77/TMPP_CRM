using System;
using System.Collections.Generic;

namespace TMPP_CRM.Domain.Builders
{
    /// <summary>
    /// Produsul final construit de OfferBuilder.
    /// Reprezintă o ofertă comercială în sistemul CRM.
    /// </summary>
    public class Offer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public List<string> Products { get; set; } = new List<string>();
        public decimal Discount { get; set; } = 0m;
        public int ValidityDays { get; set; } = 30;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"[Oferta] {Title} | Client: {ClientName} | " +
                   $"Produse: {string.Join(", ", Products)} | " +
                   $"Discount: {Discount}% | Valabilitate: {ValidityDays} zile";
        }
    }
}
