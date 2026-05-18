namespace TMPP_CRM.Domain.Strategy
{
    public interface IDiscountStrategy
    {
        decimal CalculateDiscount(decimal amount);
    }
}
