using System;
using TMPP_CRM.Domain.Common;

namespace TMPP_CRM.Domain.Entities
{
    /// <summary>Memento pattern — snapshot de Lead salvat în DB pentru restore</summary>
    public class LeadSnapshot : BaseEntity
    {
        public Guid   LeadId    { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName  { get; set; } = string.Empty;
        public string Email     { get; set; } = string.Empty;
        public string Phone     { get; set; } = string.Empty;
        public string Status    { get; set; } = string.Empty;
        public string Label     { get; set; } = "Snapshot";
    }
}
