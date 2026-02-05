using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPP_CRM.Domain.Entities;

namespace TMPP_CRM.Application.Interfaces
{
    public interface ILeadService
    {
        Task<IEnumerable<Lead>> GetAllLeadsAsync();
        Task<Lead?> GetLeadByIdAsync(Guid id);
        Task CreateLeadAsync(Lead lead);
        Task UpdateLeadStatusAsync(Guid id, string status);
        Task<Client?> ConvertLeadToClientAsync(Guid leadId);
    }
}
