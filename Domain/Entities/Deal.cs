using System;
using System.ComponentModel.DataAnnotations;
using TMPP_CRM.Domain.Common;
using TMPP_CRM.Domain.Enums;

namespace TMPP_CRM.Domain.Entities
{
    public class Deal : BaseEntity
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public decimal Value { get; set; }
        
        public DealStage Stage { get; set; } = DealStage.New;
        
        public Guid ClientId { get; set; }
        public Client? Client { get; set; }
    }
}
