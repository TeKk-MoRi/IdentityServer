using IdentityServer.Traditional.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Base
{
    public class BaseService<T, TType> : IBaseService<T, TType>
        where T : class
        where TType : IComparable
    {
        protected readonly IdentityContext _dbContext;

        protected BaseService(IdentityContext context)
        {
            _dbContext = context;
        }

        public DbSet<T> Entities => _dbContext.Set<T>();

        public async virtual Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public async virtual Task<T> AddAndSaveAsync(T entity)
        {
            var res = await _dbContext.Set<T>().AddAsync(entity);

            int result = await SaveChangesAsync();

            if (result == 0)
            {
                return null;
            }
            _dbContext.Entry(entity).State = EntityState.Detached;

            return res.Entity;
        }

        public virtual T AddAndSave(T entity)
        {
            var res = _dbContext.Set<T>().Add(entity);

            int result = SaveChanges();

            //TODO check return null
            if (result == 0)
            {
                return null;
            }
            return res.Entity;
        }

        public async virtual Task<List<T>> GetAllAsync()
        {
            return await _dbContext
                .Set<T>()
                .ToListAsync();
        }

        public async virtual Task<T> GetByIdAsync(TType id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async virtual Task<List<T>> GetPagedReponseAsync(int pageNumber, int pageSize)
        {
            return await _dbContext
                .Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async virtual Task<List<T>> GetPagedReponseWithNameAsync(string Name, int pageNumber, int pageSize)
        {
            return await _dbContext
                .Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public virtual Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).CurrentValues.SetValues(entity);
            return Task.CompletedTask;
        }
        public virtual void Update(T entity)
        {
            _dbContext.Update(entity);
        }

        public virtual int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public async virtual Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async virtual Task<int> UpdateAndSaveAsync(T entity)
        {
            _dbContext.Update(entity);
            var res = await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Detached;
            return res;
        }
        public virtual int UpdateAndSave(T entity)
        {
            _dbContext.Update(entity);
            return _dbContext.SaveChanges();
        }
    }

    public class BaseService<T> : BaseService<T, int>
        where T : class
    {
        protected BaseService(IdentityContext context) : base(context)
        {
        }
    }
}
