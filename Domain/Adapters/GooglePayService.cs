using System;

namespace TMPP_CRM.Domain.Adapters
{
    /// <summary>
    /// Serviciu Google Pay extern cu API INCOMPATIBIL cu IPaymentGateway.
    /// Adapter Pattern: "Adaptee" (Legacy/Third-party).
    /// </summary>
    public class GooglePayService
    {
        private readonly string _merchantId;

        public GooglePayService(string merchantId = "merchant-abc-123")
        {
            _merchantId = merchantId;
        }

        // API-ul propriu Google Pay – cu structura diferita
        public GooglePayResponse SendPayment(GooglePayRequest request)
        {
            Console.WriteLine($"[GooglePay] SendPayment: {request.PriceValue} {request.PriceCurrency} – Merchant: {_merchantId}");
            return new GooglePayResponse
            {
                PaymentToken = $"GPay_{Guid.NewGuid().ToString()[..10].ToUpper()}",
                ResultCode = "SUCCESS",
                Timestamp = DateTime.UtcNow
            };
        }

        public string CheckStatus(string paymentToken)
        {
            Console.WriteLine($"[GooglePay] CheckStatus: {paymentToken}");
            return paymentToken.StartsWith("GPay_") ? "COMPLETED" : "UNKNOWN";
        }
    }

    public class GooglePayRequest
    {
        public string PriceValue { get; set; } = string.Empty;
        public string PriceCurrency { get; set; } = "RON";
        public string Description { get; set; } = string.Empty;
    }

    public class GooglePayResponse
    {
        public string PaymentToken { get; set; } = string.Empty;
        public string ResultCode { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
