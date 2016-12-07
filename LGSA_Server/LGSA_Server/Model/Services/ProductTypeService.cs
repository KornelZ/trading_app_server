using LGSA.Model.UnitOfWork;
using LGSA_Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LGSA.Model.Services
{
    public class ProductTypeService : IDictionaryService<dic_Product_type>
    {
        private IUnitOfWorkFactory _factory;
        public ProductTypeService(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }
        public async Task<IEnumerable<dic_Product_type>> GetData(Expression<Func<dic_Product_type, bool>> filter)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    var entities = await unitOfWork.ProductTypeRepository.GetData(filter);

                    return entities;
                }
                catch (Exception e)
                {
                }
            }

            return null;
        }
    }
}
