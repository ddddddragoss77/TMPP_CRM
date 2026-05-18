using System.Collections.Generic;

namespace TMPP_CRM.Domain.Memento
{
    public class LeadHistory
    {
        private readonly Stack<LeadMemento> _history = new Stack<LeadMemento>();

        public void Save(LeadMemento memento)
        {
            _history.Push(memento);
        }

        public LeadMemento? Undo()
        {
            if (_history.Count > 0)
            {
                return _history.Pop();
            }
            return null;
        }

        public bool HasHistory => _history.Count > 0;
    }
}
