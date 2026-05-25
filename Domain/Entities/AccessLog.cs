using TMPP_CRM.Domain.Common;

namespace TMPP_CRM.Domain.Entities
{
    /// <summary>Proxy pattern — fiecare tentativă de acces, loghată în DB</summary>
    public class AccessLog : BaseEntity
    {
        public string  Username    { get; set; } = string.Empty;
        public string  Role        { get; set; } = string.Empty;
        public string  Operation   { get; set; } = string.Empty; // READ / WRITE
        public bool    WasGranted  { get; set; }
        public string  Result      { get; set; } = string.Empty;
        public int     CustomerId  { get; set; }
        public decimal Amount      { get; set; }
    }
}
