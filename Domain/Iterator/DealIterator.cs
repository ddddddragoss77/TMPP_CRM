using System.Collections.Generic;
using TMPP_CRM.Domain.Entities;

namespace TMPP_CRM.Domain.Iterator
{
    public class DealIterator : IDealIterator
    {
        private readonly List<Deal> _deals;
        private int _position = 0;

        public DealIterator(List<Deal> deals)
        {
            _deals = deals;
        }

        public bool HasNext()
        {
            return _position < _deals.Count;
        }

        public Deal Next()
        {
            var deal = _deals[_position];
            _position++;
            return deal;
        }

        public Deal Current()
        {
            return _deals[_position];
        }
    }
}
