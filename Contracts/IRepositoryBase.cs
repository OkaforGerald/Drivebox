using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryBase<T>
    {
        void Create(T entity);

        void Delete(T entity);
        
        void DeleteMultiple(List<T> entities);

        void Update(T entity);

        IQueryable<T> FindAll(bool trackChanges);

        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges);
    }
}
