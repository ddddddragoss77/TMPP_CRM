namespace TMPP_CRM.Domain.Proxy
{
    public interface ISensitiveResource
    {
        string GetCustomerData(int customerId);
        string UpdateFinancialRecords(int recordId, decimal amount);
    }
}
