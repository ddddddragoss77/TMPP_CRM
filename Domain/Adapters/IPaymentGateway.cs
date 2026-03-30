namespace TMPP_CRM.Domain.Adapters
{
    /// <summary>
    /// Interfata comuna pentru toate gateway-urile de plata.
    /// Adapter Pattern: aceasta este "Target Interface" pe care clientul o cunoaste.
    /// </summary>
    public interface IPaymentGateway
    {
        /// <summary>Returneaza numele providerului de plata.</summary>
        string GetProviderName();

        /// <summary>
        /// Proceseaza o plata. Returneaza un PaymentResult cu statusul si transaction ID.
        /// </summary>
        PaymentResult ProcessPayment(decimal amount, string currency, string description);

        /// <summary>Valideaza o tranzactie existenta dupa ID.</summary>
        bool ValidateTransaction(string transactionId);
    }

    /// <summary>Rezultatul unei operatii de plata.</summary>
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}
