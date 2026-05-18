namespace TMPP_CRM.Domain.Strategy
{
    public class PercentageDiscountStrategy : IDiscountStrategy
    {
        private readonly decimal _percentage;

        public PercentageDiscountStrategy(decimal percentage)
        {
            _percentage = percentage;
        }

        public decimal CalculateDiscount(decimal amount)
        {
            return amount - (amount * _percentage / 100m);
        }
    }
}
