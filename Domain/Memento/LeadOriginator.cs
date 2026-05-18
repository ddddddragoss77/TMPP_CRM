using TMPP_CRM.Domain.Entities;

namespace TMPP_CRM.Domain.Memento
{
    public static class LeadOriginator
    {
        public static LeadMemento SaveState(this Lead lead)
        {
            return new LeadMemento(lead.FirstName, lead.LastName, lead.Email, lead.Phone, lead.Status);
        }

        public static void RestoreState(this Lead lead, LeadMemento memento)
        {
            if (memento == null) return;
            
            lead.FirstName = memento.FirstName;
            lead.LastName = memento.LastName;
            lead.Email = memento.Email;
            lead.Phone = memento.Phone;
            lead.Status = memento.Status;
        }
    }
}
