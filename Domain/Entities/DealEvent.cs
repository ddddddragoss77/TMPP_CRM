using System;
using TMPP_CRM.Domain.Common;

namespace TMPP_CRM.Domain.Entities
{
    /// <summary>Observer pattern — fiecare schimbare de stage pe un deal, loghată în DB</summary>
    public class DealEvent : BaseEntity
    {
        public Guid   DealId    { get; set; }
        public string DealTitle { get; set; } = string.Empty;
        public string NewStage  { get; set; } = string.Empty;
        public string EventLog  { get; set; } = string.Empty;
    }
}
