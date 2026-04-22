namespace TMPP_CRM.Domain.Adapters
{
    /// <summary>
    /// Adaptor pentru Google Pay: traduce IPaymentGateway → GooglePayService.
    /// Construieste obiectul GooglePayRequest din parametrii simpli.
    /// </summary>
    public class GooglePayAdapter : IPaymentGateway
    {
        private readonly GooglePayService _googlePayService;

        public GooglePayAdapter(GooglePayService? googlePayService = null)
        {
            _googlePayService = googlePayService ?? new GooglePayService();
        }

        public string GetProviderName() => "Google Pay";

        public PaymentResult ProcessPayment(decimal amount, string currency, string description)
        {
            try
            {
                // Traduce: parametri simpli → obiect GooglePayRequest
                var request = new GooglePayRequest
                {
                    PriceValue = amount.ToString("F2"),
                    PriceCurrency = currency,
                    Description = description
                };

                var response = _googlePayService.SendPayment(request);

                return new PaymentResult
                {
                    IsSuccess = response.ResultCode == "SUCCESS",
                    TransactionId = response.PaymentToken,
                    Message = $"Google Pay: {response.ResultCode}",
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
            var status = _googlePayService.CheckStatus(transactionId);
            return status == "COMPLETED";
        }
    }
}
