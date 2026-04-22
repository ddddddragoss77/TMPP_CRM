namespace TMPP_CRM.Domain.Adapters
{
    /// <summary>
    /// Adaptor pentru Stripe: traduce IPaymentGateway → StripeService.
    /// Stripe lucreaza in CENTI, deci amount * 100 este necesara.
    /// </summary>
    public class StripeAdapter : IPaymentGateway
    {
        private readonly StripeService _stripeService;

        public StripeAdapter(StripeService? stripeService = null)
        {
            _stripeService = stripeService ?? new StripeService();
        }

        public string GetProviderName() => "Stripe";

        public PaymentResult ProcessPayment(decimal amount, string currency, string description)
        {
            try
            {
                // Traduce: amount in RON → centi (Stripe lucreaza in unitati minime)
                long amountInCents = (long)(amount * 100);
                var response = _stripeService.ChargeCard(amountInCents, currency.ToLower(), description);

                return new PaymentResult
                {
                    IsSuccess = response.Status == "succeeded",
                    TransactionId = response.Id,
                    Message = $"Plata Stripe: {response.Status}",
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
            return _stripeService.ConfirmCharge(transactionId);
        }
    }
}
