using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FomMonitoringCore.Repository.MongoDb
{
    public interface IGenericRepository<T> where T : class
    {
        T Find(object id);
        bool Update(T model);
        void Create(T model);
        bool Delete(object id);
        IEnumerable<T> Query();
        IEnumerable<T> Query(Expression<Func<T, bool>> filter);
    }
}