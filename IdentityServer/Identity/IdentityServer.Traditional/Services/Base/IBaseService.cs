using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Base
{
    public interface IBaseService<T, TType>
        where T : class
        where TType : IComparable
    {
        Microsoft.EntityFrameworkCore.DbSet<T> Entities { get; }

        Task<T> GetByIdAsync(TType id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetPagedReponseAsync(int pageNumber, int pageSize);
        Task<List<T>> GetPagedReponseWithNameAsync(string Name, int pageNumber, int pageSize);
        Task<T> AddAsync(T entity);
        Task<T> AddAndSaveAsync(T entity);
        T AddAndSave(T entity);
        Task UpdateAsync(T entity);
        void Update(T entity);
        Task<int> UpdateAndSaveAsync(T entity);
        int UpdateAndSave(T entity);
        public int SaveChanges();
        Task<int> SaveChangesAsync();
    }

    public interface IBaseService<T> : IBaseService<T, int>
        where T : class
    {
    }
}
