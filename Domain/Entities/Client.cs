using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TMPP_CRM.Domain.Common;

namespace TMPP_CRM.Domain.Entities
{
    public class Client : BaseEntity
    {
        [Required]
        public string CompanyName { get; set; } = string.Empty;
        
        public string? ContactName { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        [Phone]
        public string? Phone { get; set; }
        
        public List<Deal> Deals { get; set; } = new List<Deal>();
    }
}
