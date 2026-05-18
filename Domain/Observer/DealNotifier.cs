using System.Collections.Generic;
using TMPP_CRM.Domain.Entities;

namespace TMPP_CRM.Domain.Observer
{
    public class DealNotifier : IDealSubject
    {
        private readonly List<IDealObserver> _observers = new List<IDealObserver>();
        private Deal _deal;

        public DealNotifier(Deal deal)
        {
            _deal = deal;
        }

        public void ChangeDealStage(TMPP_CRM.Domain.Enums.DealStage newStage)
        {
            _deal.Stage = newStage;
            Notify();
        }

        public void Attach(IDealObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IDealObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.Update(_deal);
            }
        }
    }
}
