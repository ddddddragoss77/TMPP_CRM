using TMPP_CRM.Domain.Common;

namespace TMPP_CRM.Domain.Entities
{
    /// <summary>Adapter pattern — tranzacție de plată salvată în DB</summary>
    public class Transaction : BaseEntity
    {
        public string  Provider       { get; set; } = string.Empty;
        public decimal Amount         { get; set; }
        public string  Currency       { get; set; } = "RON";
        public string  Description    { get; set; } = string.Empty;
        public bool    IsSuccess      { get; set; }
        public string  TransactionRef { get; set; } = string.Empty;
        public string  Result         { get; set; } = string.Empty;
    }
}
