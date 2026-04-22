using System;

namespace TMPP_CRM.Domain.Proxy
{
    public class ResourceProxy : ISensitiveResource
    {
        private RealResource _realResource;
        private readonly UserSession _session;

        public ResourceProxy(UserSession session)
        {
            _session = session;
        }

        private void EnsureRealResourceCreated()
        {
            if (_realResource == null)
            {
                _realResource = new RealResource();
            }
        }

        public string GetCustomerData(int customerId)
        {
            if (_session.Role == "Admin" || _session.Role == "Manager" || _session.Role == "Agent")
            {
                EnsureRealResourceCreated();
                return _realResource.GetCustomerData(customerId);
            }
            throw new UnauthorizedAccessException("Access denied. Insufficient privileges to view customer data.");
        }

        public string UpdateFinancialRecords(int recordId, decimal amount)
        {
            if (_session.Role == "Admin" || _session.Role == "Manager")
            {
                EnsureRealResourceCreated();
                return _realResource.UpdateFinancialRecords(recordId, amount);
            }
            throw new UnauthorizedAccessException("Access denied. Only Admins and Managers can update financial records.");
        }
    }
}
