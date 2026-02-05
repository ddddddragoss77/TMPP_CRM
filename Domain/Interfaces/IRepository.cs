using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPP_CRM.Domain.Common;

namespace TMPP_CRM.Domain.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }
}
