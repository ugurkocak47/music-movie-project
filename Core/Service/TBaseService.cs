using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Core.Entity;

namespace Core.Service
{
    public interface ITBaseService<T, TId, TAppUser, TDbContext> where T : IEntity<TId> where TAppUser : class where TDbContext : DbContext, new()
    {
        Task<T> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        T Add(T entity);
        bool Update(T entity);
        bool Delete(T entity);
        // queryable
        IQueryable<T> GetAll();
        IList<T> GetAllList(Expression<Func<T, bool>>? predicate = null);
        Task<IList<T>> GetAllListAsync(Expression<Func<T, bool>>? predicate = null);
        T FirstOrDefault(Expression<Func<T, bool>>? predicate = null);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = null);

    }

    public class TBaseService<T, TId, TAppUser,TDbContext> : ITBaseService<T, TId, TAppUser, TDbContext> where T : IEntity<TId> where TAppUser : class where TDbContext : DbContext, new()
    {
        private readonly UserManager<TAppUser> _userManager;
        private readonly TDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TBaseService(TDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<TAppUser> userManager)
        {
            _userManager = userManager;
            _context = context;
            _dbSet = context.Set<T>();
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<T> AddAsync(T entity)
        {
            if (_httpContextAccessor.HttpContext?.User != null)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                entity.CreatedBy = Guid.Parse(userId);
            }
            entity.CreatedDate = DateTime.Now;
            entity.UpdatedDate = DateTime.Now; 
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                throw new UnauthorizedAccessException("Bu işlemi yapabilmek için giriş yapmanız gerekiyor.");
            }
            var userId = _userManager.GetUserId(user);
            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedBy = Guid.Parse(userId!);
            var existingEntity = await _dbSet.FindAsync(entity.Id);

            if (existingEntity != null)
            {
                foreach (var property in typeof(T).GetProperties())
                {
                    var newValue = property.GetValue(entity);
                    if (newValue != null)
                    {
                        property.SetValue(existingEntity, newValue);
                    }
                }
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            if (_httpContextAccessor.HttpContext?.User != null)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                entity.UpdatedBy = Guid.Parse(userId);
            }
            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.Now;
            _dbSet.Update(entity);
            try {
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                return false;
            }
            return true;
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable().Where(y => y.IsDeleted == false);
        }

        public IList<T> GetAllList(Expression<Func<T, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public async Task<IList<T>> GetAllListAsync(Expression<Func<T, bool>> predicate = null)
        {
            return  predicate == null 
                ? await GetAll().ToListAsync() 
                : await  GetAll().Where(predicate).ToListAsync();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetAll().FirstOrDefaultAsync(predicate);
        }

        public T Add(T entity)
        {
            if (_httpContextAccessor.HttpContext?.User != null)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                entity.CreatedBy = Guid.Parse(userId);
            }
            entity.CreatedDate = DateTime.Now;
            entity.UpdatedDate = DateTime.Now;
            _dbSet.Add(entity);
            try {
                _context.SaveChanges();
            } catch (Exception ex) {
                return null;
            }
            return entity;
        }

        public bool Update(T entity)
        {
            if (_httpContextAccessor.HttpContext?.User != null)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                entity.UpdatedBy = Guid.Parse(userId);
            }
            entity.UpdatedDate = DateTime.Now;
            _dbSet.Update(entity);
            try {
                _context.SaveChanges();
            } catch (Exception ex) {
                return false;
            }
            return true;
        }

        public bool Delete(T entity)
        {
            if (_httpContextAccessor.HttpContext?.User != null)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                entity.UpdatedBy = Guid.Parse(userId);
            }
            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.Now;
            _dbSet.Update(entity);
            try {
                _context.SaveChanges();
            } catch (Exception ex) {
                return false;
            }
            return true;
        }
    }
}