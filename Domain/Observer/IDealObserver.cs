using TMPP_CRM.Domain.Entities;

namespace TMPP_CRM.Domain.Observer
{
    public interface IDealObserver
    {
        void Update(Deal deal);
    }
}
