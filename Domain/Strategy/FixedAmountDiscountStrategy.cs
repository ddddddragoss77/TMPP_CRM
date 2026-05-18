namespace TMPP_CRM.Domain.Strategy
{
    public class FixedAmountDiscountStrategy : IDiscountStrategy
    {
        private readonly decimal _discountAmount;

        public FixedAmountDiscountStrategy(decimal discountAmount)
        {
            _discountAmount = discountAmount;
        }

        public decimal CalculateDiscount(decimal amount)
        {
            return amount > _discountAmount ? amount - _discountAmount : 0;
        }
    }
}
