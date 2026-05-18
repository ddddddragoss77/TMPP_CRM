namespace TMPP_CRM.Domain.Memento
{
    public class LeadMemento
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Phone { get; }
        public string Status { get; }

        public LeadMemento(string firstName, string lastName, string email, string phone, string status)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            Status = status;
        }
    }
}
