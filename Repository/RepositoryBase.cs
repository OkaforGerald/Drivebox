using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext context;

        public RepositoryBase(RepositoryContext context)
        {
            this.context = context;
        }

        public void Create(T entity)
        {
            context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        public IQueryable<T> FindAll(bool trackChanges)
        {
            return !trackChanges ? context.Set<T>().AsNoTracking() : context.Set<T>();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
        {
            return !trackChanges ? context.Set<T>().Where(expression).AsNoTracking() : context.Set<T>().Where(expression);
        }

        public void Update(T entity)
        {
            context.Set<T>().Update(entity);
        }
    }
}
