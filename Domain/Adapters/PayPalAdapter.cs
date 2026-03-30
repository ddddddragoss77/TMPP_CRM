namespace TMPP_CRM.Domain.Adapters
{
    /// <summary>
    /// Adaptor pentru PayPal: traduce IPaymentGateway → PayPalService.
    /// "Wraps" PayPalService pentru a-l face compatibil cu interfata comuna.
    /// </summary>
    public class PayPalAdapter : IPaymentGateway
    {
        private readonly PayPalService _payPalService;

        public PayPalAdapter(PayPalService? payPalService = null)
        {
            _payPalService = payPalService ?? new PayPalService();
        }

        public string GetProviderName() => "PayPal";

        public PaymentResult ProcessPayment(decimal amount, string currency, string description)
        {
            try
            {
                // Traduce: decimal → double (conversia necesara pentru API-ul PayPal)
                var transactionId = _payPalService.MakePayment((double)amount, currency, description);

                return new PaymentResult
                {
                    IsSuccess = true,
                    TransactionId = transactionId,
                    Message = $"Plata PayPal procesata cu succes.",
                    Amount = amount,
                    Currency = currency,
                    Provider = GetProviderName()
                };
            }
            catch (Exception ex)
            {
                return new PaymentResult { IsSuccess = false, Message = ex.Message, Provider = GetProviderName() };
            }
        }

        public bool ValidateTransaction(string transactionId)
        {
            return _payPalService.VerifyTransaction(transactionId);
        }
    }
}
