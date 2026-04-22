using System.Globalization;

namespace TMPP_CRM.Domain.Proxy
{
    public class RealResource : ISensitiveResource
    {
        public string GetCustomerData(int customerId)
        {
            return $"Returning sensitive data for customer {customerId}";
        }

        public string UpdateFinancialRecords(int recordId, decimal amount)
        {
            return $"Financial record {recordId} updated with amount {amount.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}
