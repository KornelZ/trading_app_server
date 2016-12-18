using LGSA_Server.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LGSA.Model.Services
{
    public interface IDictionaryService<T>
    {
        Task<IEnumerable<T>> GetData(Expression<Func<T, bool>> filter);
    }
    public interface IDataService<T> : IDictionaryService<T>
    {
        Task<T> GetById(int id);
        Task<ErrorValue> Add(T entity);
        Task<ErrorValue> Update(T entity);
        Task<ErrorValue> Delete(T entity);
    }
}
