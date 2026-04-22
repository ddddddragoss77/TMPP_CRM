using System;

namespace TMPP_CRM.Domain.Adapters
{
    /// <summary>
    /// Serviciu PayPal extern cu API INCOMPATIBIL cu IPaymentGateway.
    /// Adapter Pattern: aceasta este clasa "Adaptee" (Legacy/Third-party).
    /// Nu poate fi modificata direct.
    /// </summary>
    public class PayPalService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;

        public PayPalService(string clientId = "paypal-client-id", string clientSecret = "paypal-secret")
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        // API-ul propriu PayPal – incompatibil cu IPaymentGateway
        public string MakePayment(double amount, string currencyCode, string note)
        {
            // Simuleaza trimiterea platii catre PayPal
            Console.WriteLine($"[PayPal] MakePayment: {amount} {currencyCode} – {note}");
            return $"PP-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }

        public bool VerifyTransaction(string paypalTransactionId)
        {
            Console.WriteLine($"[PayPal] VerifyTransaction: {paypalTransactionId}");
            return paypalTransactionId.StartsWith("PP-");
        }

        public string GetPayPalAccountInfo()
        {
            return $"PayPal Account [Client: {_clientId}]";
        }
    }
}
