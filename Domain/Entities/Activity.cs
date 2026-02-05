using System;
using System.ComponentModel.DataAnnotations;
using TMPP_CRM.Domain.Common;

namespace TMPP_CRM.Domain.Entities
{
    public abstract class Activity : BaseEntity
    {
        public DateTime DueDate { get; set; }
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        public bool IsCompleted { get; set; }
    }

    public class CallActivity : Activity
    {
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class EmailActivity : Activity
    {
        [Required]
        public string Subject { get; set; } = string.Empty;
        
        [EmailAddress]
        public string ToEmail { get; set; } = string.Empty;
    }
}
