using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BlogService.API.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    namespace BlogService.API.Data.Repositories
    {
        public interface IRepository<T> where T : class
        {
            Task<T> GetByIdAsync(int id);
            Task<IEnumerable<T>> GetAllAsync();
            Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
            IQueryable<T> Query();
            Task AddAsync(T entity);
            void Update(T entity);
            void Delete(T entity);
            Task SaveChangesAsync();
            Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        }
    }
}