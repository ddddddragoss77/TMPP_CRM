using System;
using TMPP_CRM.Domain.Common;

namespace TMPP_CRM.Domain.Entities
{
    /// <summary>Command pattern — fiecare execute/undo, salvat în DB ca audit trail</summary>
    public class CommandHistoryEntry : BaseEntity
    {
        public Guid   DealId        { get; set; }
        public string DealTitle     { get; set; } = string.Empty;
        public string Action        { get; set; } = string.Empty; // CREATE / EXECUTE / UNDO
        public string PreviousStage { get; set; } = string.Empty;
        public string NewStage      { get; set; } = string.Empty;
        public string Actor         { get; set; } = string.Empty;
    }
}
