using Microsoft.EntityFrameworkCore;
using Core.Entity;


namespace Core.Service;

public interface IDbOperationEvent<T, TId, TAppUser, TDbContext> where T : IEntity<TId> where TAppUser : class where TDbContext : DbContext, new()
{
    ITBaseService<T,TId, TAppUser, TDbContext> Current { get; }
}