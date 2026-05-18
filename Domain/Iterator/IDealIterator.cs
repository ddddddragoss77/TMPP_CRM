using TMPP_CRM.Domain.Entities;

namespace TMPP_CRM.Domain.Iterator
{
    public interface IDealIterator
    {
        bool HasNext();
        Deal Next();
        Deal Current();
    }
}
