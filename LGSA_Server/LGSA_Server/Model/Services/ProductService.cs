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
                    NullProperties(entity);
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
                    NullProperties(entity);
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
        protected virtual async Task<bool> CanUpdate(product entity, IUnitOfWork unitOfWork)
        {
            var offers = await unitOfWork.SellOfferRepository.GetData(b => b.product_id == entity.ID && b.status_id != 3);
            var totalAmount = offers.Sum(b => b.amount);
            if(totalAmount > entity.stock)
            {
                return false;
            }
            return true;
        }
        public virtual async Task<ErrorValue> Update(product entity)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    NullProperties(entity);
                    var prod = await unitOfWork.ProductRepository.GetById(entity.ID);
                    if(prod == null)
                    {
                        return ErrorValue.ServerError;
                    }
                    
                    var canUpdate = await CanUpdate(entity, unitOfWork);
                    if(canUpdate == false)
                    {
                        return ErrorValue.AmountGreaterThanStock;
                    }
                    prod.condition_id = entity.condition_id;
                    prod.genre_id = entity.genre_id;
                    prod.rating = entity.rating;
                    prod.sold_copies = entity.sold_copies;
                    prod.stock = entity.stock;
                    prod.Update_Date = DateTime.Now;
                    prod.product_type_id = entity.product_type_id;
                    prod.Name = entity.Name;
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
        protected virtual void NullProperties(product entity)
        {
            entity.dic_condition = null;
            entity.dic_Genre = null;
            entity.dic_Product_type = null;
            entity.users = null;
            entity.users1 = null;
        }

    }
    
}
