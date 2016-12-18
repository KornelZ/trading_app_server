using LGSA.Model.UnitOfWork;
using LGSA_Server.Model;
using LGSA_Server.Model.Enums;
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
    public class ProductService : IDataService<product>
    {
        private IUnitOfWorkFactory _factory;
        public ProductService(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }

        public async Task<ErrorValue> Add(product entity)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    var result = await unitOfWork.ProductRepository.GetData(p => p.product_owner == entity.product_owner && p.Name == entity.Name);
                    if(result.Count() != 0)
                    {
                        return ErrorValue.EntityExists;
                    }
                    unitOfWork.ProductRepository.Add(entity);
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    return ErrorValue.ServerError;
                }
            }
            return ErrorValue.NoError;
        }

        public async Task<ErrorValue> Delete(product entity)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    unitOfWork.ProductRepository.Delete(entity);
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    return ErrorValue.ServerError;
                }
            }
            return ErrorValue.NoError;
        }

        public async Task<product> GetById(int id)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    product entity = await unitOfWork.ProductRepository.GetById(id);
                    return entity;
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                }
            }
            return null;
        }

        public virtual async Task<IEnumerable<product>> GetData(Expression<Func<product, bool>> filter)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    var entities = await unitOfWork.ProductRepository.GetData(filter);

                    return entities;
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        public virtual async Task<ErrorValue> Update(product entity)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    unitOfWork.ProductRepository.Update(entity);
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    return ErrorValue.ServerError;
                }
            }
            return ErrorValue.NoError;
        }
    }
    
}
