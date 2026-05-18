using System.Collections.Generic;
using TMPP_CRM.Domain.Entities;

namespace TMPP_CRM.Domain.Iterator
{
    public class DealCollection : IDealCollection
    {
        private readonly List<Deal> _deals = new List<Deal>();

        public void AddDeal(Deal deal)
        {
            _deals.Add(deal);
        }

        public void RemoveDeal(Deal deal)
        {
            _deals.Remove(deal);
        }

        public int Count => _deals.Count;

        public IDealIterator CreateIterator()
        {
            return new DealIterator(_deals);
        }
    }
}
