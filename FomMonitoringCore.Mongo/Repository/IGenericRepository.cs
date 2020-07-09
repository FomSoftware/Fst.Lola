using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FomMonitoringCore.Mongo.Dto;

namespace FomMonitoringCore.Mongo.Repository
{
    public interface IGenericRepository<T> where T : BaseModel
    {
        T Find(string id);
        bool Update(T model);
        void Create(T model);
        bool Delete(object id);
        IEnumerable<T> Query();
        IEnumerable<T> Query(Expression<Func<T, bool>> filter);
    }
}