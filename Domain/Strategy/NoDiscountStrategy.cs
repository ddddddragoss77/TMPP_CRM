namespace TMPP_CRM.Domain.Strategy
{
    public class NoDiscountStrategy : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal amount)
        {
            return amount;
        }
    }
}
