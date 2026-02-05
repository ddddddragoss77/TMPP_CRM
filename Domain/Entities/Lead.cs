using System.ComponentModel.DataAnnotations;
using TMPP_CRM.Domain.Common;

namespace TMPP_CRM.Domain.Entities
{
    public class Lead : BaseEntity
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Phone]
        public string Phone { get; set; } = string.Empty;
        
        public string Status { get; set; } = "New";

        public Client ConvertToClient()
        {
            return new Client
            {
                ContactName = $"{FirstName} {LastName}",
                Email = Email,
                Phone = Phone,
                CompanyName = $"{LastName} Inc." // Placeholder default
            };
        }
    }
}
