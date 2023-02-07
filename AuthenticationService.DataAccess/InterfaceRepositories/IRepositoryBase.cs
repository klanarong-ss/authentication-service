using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.DataAccess.InterfaceRepositories
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> AddRange(IEnumerable<T> entity);
        Task<bool> RemoveRange(IEnumerable<T> entity);
        Task<bool> Any(Expression<Func<T, bool>> predicate);
    }
}
