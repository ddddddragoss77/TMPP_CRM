using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TMPP_CRM.Domain.Common;
using TMPP_CRM.Domain.Interfaces;
using TMPP_CRM.Infrastructure.Data;

namespace TMPP_CRM.Infrastructure.Persistence
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly CrmDbContext _db;
        private readonly DbSet<T> _set;

        public EfRepository(CrmDbContext db)
        {
            _db  = db;
            _set = db.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() =>
            await _set.ToListAsync();

        public async Task<T?> GetByIdAsync(Guid id) =>
            await _set.FindAsync(id);

        public async Task AddAsync(T entity)
        {
            await _set.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _set.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _set.FindAsync(id);
            if (entity != null)
            {
                _set.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}
