using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;
using Core.Entity;

namespace Core.Service
{
    public class DbOperationService<T, TId, TAppUser, TDbContext> where T : IEntity<TId> where TAppUser : class where TDbContext : DbContext, new()
    {
        private readonly ITBaseService<T,TId,TAppUser,TDbContext> _service;
        public DbOperationService(ITBaseService<T, TId, TAppUser, TDbContext> service)
        {
            _service = service;
        }
        public async Task<T> AddAsync(T entity)
        {
            return await _service.AddAsync(entity);
        }
        public async Task<bool> UpdateAsync(T entity)
        {
            return await _service.UpdateAsync(entity);
        }
        public async Task<bool> DeleteAsync(T entity)
        {
            return await _service.DeleteAsync(entity);
        }
        public T Add(T entity)
        {
            return _service.Add(entity);
        }
        public bool Update(T entity)
        {
            return _service.Update(entity);
        }
        public bool Delete(T entity)
        {
            return _service.Delete(entity);
        }
        public IQueryable<T> GetAll()
        {
            return _service.GetAll();
        }
        public IList<T> GetAllList(Expression<Func<T, bool>> predicate)
        {
            return _service.GetAllList(predicate);
        }
        public async Task<IList<T>> GetAllListAsync(Expression<Func<T, bool>> predicate)
        {
            return await _service.GetAllListAsync(predicate);
        }
        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _service.FirstOrDefault(predicate);
        }
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _service.FirstOrDefaultAsync(predicate);
        }
    }
}