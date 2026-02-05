using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPP_CRM.Application.Interfaces;
using TMPP_CRM.Domain.Entities;
using TMPP_CRM.Domain.Interfaces;

namespace TMPP_CRM.Application.Services
{
    public class LeadService : ILeadService
    {
        private readonly IRepository<Lead> _leadRepository;
        private readonly IRepository<Client> _clientRepository;

        public LeadService(IRepository<Lead> leadRepository, IRepository<Client> clientRepository)
        {
            _leadRepository = leadRepository;
            _clientRepository = clientRepository;
        }

        public async Task<IEnumerable<Lead>> GetAllLeadsAsync()
        {
            return await _leadRepository.GetAllAsync();
        }

        public async Task<Lead?> GetLeadByIdAsync(Guid id)
        {
            return await _leadRepository.GetByIdAsync(id);
        }

        public async Task CreateLeadAsync(Lead lead)
        {
            // Business logic/validation could go here
            await _leadRepository.AddAsync(lead);
        }

        public async Task UpdateLeadStatusAsync(Guid id, string status)
        {
            var lead = await _leadRepository.GetByIdAsync(id);
            if (lead != null)
            {
                lead.Status = status;
                await _leadRepository.UpdateAsync(lead);
            }
        }

        public async Task<Client?> ConvertLeadToClientAsync(Guid leadId)
        {
            var lead = await _leadRepository.GetByIdAsync(leadId);
            if (lead == null) return null;

            var client = lead.ConvertToClient();
            await _clientRepository.AddAsync(client);
            
            // Optionally remove lead or mark as converted
            lead.Status = "Converted";
            await _leadRepository.UpdateAsync(lead);

            return client;
        }
    }
}
