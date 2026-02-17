using System;

namespace TMPP_CRM.Domain.Entities
{
    public abstract class Activity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

        public abstract string GetActivityType();
    }
}
