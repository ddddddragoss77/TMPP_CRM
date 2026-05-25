using TMPP_CRM.Domain.Common;

namespace TMPP_CRM.Domain.Entities
{
    /// <summary>Decorator pattern — fiecare notificare trimisă, loghată în DB</summary>
    public class NotificationLog : BaseEntity
    {
        public string Message    { get; set; } = string.Empty;
        public bool   EmailSent  { get; set; } = true;
        public bool   SmsSent    { get; set; }
        public bool   PushSent   { get; set; }
        public int    LayerCount { get; set; } = 1;
        public string Result     { get; set; } = string.Empty;
    }
}
