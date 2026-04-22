using System;

namespace TMPP_CRM.Domain.Adapters
{
    /// <summary>
    /// Serviciu Stripe extern cu API INCOMPATIBIL cu IPaymentGateway.
    /// Adapter Pattern: "Adaptee" (Legacy/Third-party).
    /// </summary>
    public class StripeService
    {
        private readonly string _apiKey;

        public StripeService(string apiKey = "sk_test_stripe_key")
        {
            _apiKey = apiKey;
        }

        // API-ul propriu Stripe – metode diferite de IPaymentGateway
        public StripeChargeResponse ChargeCard(long amountInCents, string currency, string statementDescriptor)
        {
            Console.WriteLine($"[Stripe] ChargeCard: {amountInCents} cents {currency.ToUpper()} – {statementDescriptor}");
            return new StripeChargeResponse
            {
                Id = $"ch_{Guid.NewGuid().ToString()[..14]}",
                Status = "succeeded",
                AmountCents = amountInCents
            };
        }

        public bool ConfirmCharge(string chargeId)
        {
            Console.WriteLine($"[Stripe] ConfirmCharge: {chargeId}");
            return chargeId.StartsWith("ch_");
        }

        public string GetStripePublicKey() => $"pk_test_{_apiKey.GetHashCode():X8}";
    }

    public class StripeChargeResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public long AmountCents { get; set; }
    }
}
