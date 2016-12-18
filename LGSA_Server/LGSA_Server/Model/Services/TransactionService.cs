using LGSA.Model.UnitOfWork;
using LGSA_Server.Model;
using LGSA_Server.Model.Enums;
using LGSA_Server.Model.Services.TransactionLogic;
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

    public interface ITransactionService : IDataService<transactions>
    {
        Task<ErrorValue> AcceptSellTransaction(sell_Offer sellOffer, buy_Offer buyOffer, int? rating);
        Task<ErrorValue> AcceptBuyTransaction(sell_Offer sellOffer, buy_Offer buyOffer, int? rating);

    }
    public class TransactionService : ITransactionService
    {
        private IUnitOfWorkFactory _factory;
        private IRatingUpdater _ratingUpdater;
        public TransactionService(IUnitOfWorkFactory factory, IRatingUpdater ratingUpdater)
        {
            _factory = factory;
            _ratingUpdater = ratingUpdater;
        }
        public async Task<ErrorValue> AcceptSellTransaction(sell_Offer sellOffer, buy_Offer buyOffer, int? rating)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    sellOffer = await unitOfWork.SellOfferRepository.GetById(sellOffer.ID);
                    buyOffer.product_id = sellOffer.product_id;
                    UpdateOffers(sellOffer, buyOffer);
                    var boughtProduct = await GetBoughtProduct(sellOffer, buyOffer, unitOfWork);
                    await _ratingUpdater.UpdateRating(sellOffer.seller_id, unitOfWork, rating);
                    var transaction = new transactions()
                    {
                        buyer_id = buyOffer.buyer_id,
                        seller_id = sellOffer.seller_id,
                        buy_Offer = buyOffer,
                        sell_Offer = sellOffer,
                        status_id = (int)TransactionState.Finished,
                        transaction_Date = DateTime.Now,
                        Update_Who = buyOffer.buyer_id,
                        Update_Date = DateTime.Now,
                        Rating = rating 
                    };
                    unitOfWork.TransactionRepository.Add(transaction);
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

        private async Task<product> GetBoughtProduct(sell_Offer sellOffer, buy_Offer buyOffer, IUnitOfWork unitOfWork)
        {
            var soldProduct = await unitOfWork.ProductRepository.GetById(sellOffer.product_id);
            soldProduct.stock -= sellOffer.amount;
            unitOfWork.ProductRepository.Update(soldProduct);
            var boughtProduct = await unitOfWork.ProductRepository.GetData(p => p.product_owner == buyOffer.buyer_id
                                                                    && p.Name == soldProduct.Name);
            if(boughtProduct.Count() != 0)
            {
                var p = boughtProduct.First();
                p.stock += sellOffer.amount;
                unitOfWork.ProductRepository.Update(p);
                return p;
            }

            var product = new product()
            {
                ID = 0,
                condition_id = soldProduct.condition_id,
                genre_id = soldProduct.genre_id,
                product_type_id = soldProduct.product_type_id,
                Name = soldProduct.Name,
                Update_Date = DateTime.Now,
                Update_Who = buyOffer.buyer_id,
                stock = sellOffer.amount,
                sold_copies = 0,
                product_owner = buyOffer.buyer_id
            };
            unitOfWork.ProductRepository.Add(product);

            return product;  
        }

        private void UpdateOffers(sell_Offer sellOffer, buy_Offer buyOffer)
        {
            buyOffer.status_id = (int)TransactionState.Finished;
            sellOffer.status_id = (int)TransactionState.Finished;
        }
        public async Task<ErrorValue> AcceptBuyTransaction(sell_Offer sellOffer, buy_Offer buyOffer, int? rating)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    buyOffer = await unitOfWork.BuyOfferRepository.GetById(buyOffer.ID);
                    var soldProduct = await GetSoldProduct(sellOffer, buyOffer, unitOfWork);
                    if(soldProduct == null)
                    {
                        return ErrorValue.AmountGreaterThanStock;
                    }
                    sellOffer.product = soldProduct;
                    UpdateOffers(sellOffer, buyOffer);
                    await _ratingUpdater.UpdateRating(buyOffer.buyer_id, unitOfWork, rating);

                    var transaction = new transactions()
                    {
                        buyer_id = buyOffer.buyer_id,
                        seller_id = sellOffer.seller_id,
                        buy_Offer = buyOffer,
                        sell_Offer = sellOffer,
                        status_id = (int)TransactionState.Finished,
                        transaction_Date = DateTime.Now,
                        Update_Who = sellOffer.seller_id,
                        Update_Date = DateTime.Now,
                        Rating = rating
                    };

                    unitOfWork.TransactionRepository.Add(transaction);
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
        private async Task<product> GetSoldProduct(sell_Offer sellOffer, buy_Offer buyOffer, IUnitOfWork unitOfWork)
        {
            var boughtProduct = await unitOfWork.ProductRepository.GetById(buyOffer.product_id);
            boughtProduct.stock += sellOffer.amount;
            unitOfWork.ProductRepository.Update(boughtProduct);
            var soldProduct = await unitOfWork.ProductRepository.GetData(p => p.product_owner == sellOffer.seller_id
                                                                    && p.Name == boughtProduct.Name);
            if (soldProduct.Count() != 0)
            {
                var p = soldProduct.First();
                if(p.stock < sellOffer.amount)
                {
                    return null;
                }
                p.stock -= sellOffer.amount;
                unitOfWork.ProductRepository.Update(p);
                return p;
            }
            return null;
        }
        public async Task<ErrorValue> Add(transactions entity)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    unitOfWork.TransactionRepository.Add(entity);
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
        public async Task<ErrorValue> Delete(transactions entity)
        {
            bool success = true;
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    unitOfWork.TransactionRepository.Delete(entity);
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
        public async Task<transactions> GetById(int id)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    var entity = await unitOfWork.TransactionRepository.GetById(id);
                    return entity;
                }
                catch (Exception e)
                {
                }
            }
            return null;
        }
        public async Task<IEnumerable<transactions>> GetData(Expression<Func<transactions, bool>> filter)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    var entities = await unitOfWork.TransactionRepository.GetData(filter);

                    return entities;
                }
                catch (Exception e)
                {
                }
            }
            return null;
        }
        public async Task<ErrorValue> Update(transactions entity)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    unitOfWork.TransactionRepository.Update(entity);
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
