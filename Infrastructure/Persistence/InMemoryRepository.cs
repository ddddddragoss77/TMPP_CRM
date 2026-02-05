using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPP_CRM.Domain.Common;
using TMPP_CRM.Domain.Interfaces;

namespace TMPP_CRM.Infrastructure.Persistence
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ConcurrentDictionary<Guid, T> _store = new();

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult((IEnumerable<T>)_store.Values.ToList());
        }

        public Task<T?> GetByIdAsync(Guid id)
        {
            _store.TryGetValue(id, out var entity);
            return Task.FromResult(entity);
        }

        public Task AddAsync(T entity)
        {
            _store.TryAdd(entity.Id, entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entity)
        {
            if (_store.ContainsKey(entity.Id))
            {
                _store[entity.Id] = entity;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            _store.TryRemove(id, out _);
            return Task.CompletedTask;
        }
    }
}
