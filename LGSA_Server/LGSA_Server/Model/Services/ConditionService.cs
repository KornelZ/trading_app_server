using LGSA.Model.UnitOfWork;
using LGSA_Server.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LGSA.Model.Services
{
    public class ConditionService : IDictionaryService<dic_condition>
    {
        private IUnitOfWorkFactory _factory;
        public ConditionService(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<dic_condition>> GetData(Expression<Func<dic_condition, bool>> filter)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    var entities = await unitOfWork.ConditionRepository.GetData(filter);

                    return entities;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
