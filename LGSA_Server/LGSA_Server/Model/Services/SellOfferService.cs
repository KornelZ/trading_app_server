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
    public class SellOfferService : IDataService<sell_Offer>
    {
        private IUnitOfWorkFactory _factory;
        public SellOfferService(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }

        public async Task<ErrorValue> Add(sell_Offer entity)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    var canAdd = await CanAddOffer(entity, unitOfWork);
                    if(canAdd == false)
                    {
                        return ErrorValue.AmountGreaterThanStock;
                    }
                    unitOfWork.SellOfferRepository.Add(entity);
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch (DBConcurrencyException)
                {
                    unitOfWork.Rollback();
                    return ErrorValue.ServerError;
                }
            }
            return ErrorValue.NoError;
        }
        private async Task<bool> CanAddOffer(sell_Offer entity, IUnitOfWork unitOfWork)
        {
            var productOffers = await unitOfWork.SellOfferRepository
                                        .GetData(offer => offer.seller_id == entity.seller_id
                                        && offer.product_id == entity.product_id && offer.status_id != 3);
            var totalAmount = entity.amount + productOffers.Sum(offer => offer.amount);
            var product = await unitOfWork.ProductRepository.GetById(entity.product_id);

            if(totalAmount > product?.stock)
            {
                return false;
            }
            return true;
        }
        public async Task<ErrorValue> Delete(sell_Offer entity)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    unitOfWork.SellOfferRepository.Delete(entity);
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch (DBConcurrencyException)
                {
                    unitOfWork.Rollback();
                    return ErrorValue.ServerError;
                }
            }
            return ErrorValue.NoError;
        }

        public async Task<sell_Offer> GetById(int id)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    var entity = await unitOfWork.SellOfferRepository.GetById(id);
                    return entity;
                }
                catch (EntityException)
                {
                }
            }
            return null;
        }

        public async Task<IEnumerable<sell_Offer>> GetData(Expression<Func<sell_Offer, bool>> filter)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    var entities = await unitOfWork.SellOfferRepository.GetData(filter);

                    return entities;
                }
                catch (EntityException)
                {
                }
            }
            return null;
        }

        public async Task<ErrorValue> Update(sell_Offer entity)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    unitOfWork.SellOfferRepository.Update(entity);
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch (DBConcurrencyException)
                {
                    unitOfWork.Rollback();
                    return ErrorValue.ServerError;
                }
            }
            return ErrorValue.NoError;
        }
    }
}

