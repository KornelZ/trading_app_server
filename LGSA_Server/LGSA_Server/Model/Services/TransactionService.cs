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

        protected virtual void NullProperties(sell_Offer sellOffer, buy_Offer buyOffer)
        {
            sellOffer.dic_Offer_status = null;
            sellOffer.users1 = null;
            sellOffer.users = null;
            sellOffer.product = null;

            buyOffer.dic_Offer_status = null;
            buyOffer.product = null;
            buyOffer.users = null;
            buyOffer.users1 = null;
        }
        public async Task<ErrorValue> AcceptSellTransaction(sell_Offer sellOffer, buy_Offer buyOffer, int? rating)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    NullProperties(sellOffer, buyOffer);
                    //get actual sellOffer from database
                    sellOffer = await unitOfWork.SellOfferRepository.GetById(sellOffer.ID);
                    if(sellOffer.status_id == 3)
                    {
                        return ErrorValue.TransactionAlreadyFinished;
                    }
                    buyOffer.product_id = sellOffer.product_id;
                    //set offers to finished
                    UpdateOffers(sellOffer, buyOffer, unitOfWork);
                    //update product stocks
                    var boughtProduct = await GetBoughtProduct(sellOffer, buyOffer, unitOfWork);
                    //change user rating
                    await _ratingUpdater.UpdateRating(sellOffer.seller_id, unitOfWork, rating);
                    var transaction = new transactions()
                    {
                        buyer_id = buyOffer.buyer_id,
                        seller_id = sellOffer.seller_id,
                        buy_Offer = buyOffer,
                        sell_offer_id = sellOffer.ID,
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
                catch (Exception)
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
            //if there is an existing product with such name and owner then update
            if(boughtProduct.Count() != 0)
            {
                var p = boughtProduct.First();
                p.stock += sellOffer.amount;
                unitOfWork.ProductRepository.Update(p);
                return p;
            }
            //otherwise update and add
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

        private void UpdateOffers(sell_Offer sellOffer, buy_Offer buyOffer, IUnitOfWork unitOfWork)
        {
            buyOffer.status_id = (int)TransactionState.Finished;
            sellOffer.status_id = (int)TransactionState.Finished;

            if(buyOffer.ID != 0)
            {
                unitOfWork.BuyOfferRepository.Update(buyOffer);
            }
            else if(sellOffer.ID != 0)
            {
                unitOfWork.SellOfferRepository.Update(sellOffer);
            }
        }
        public async Task<ErrorValue> AcceptBuyTransaction(sell_Offer sellOffer, buy_Offer buyOffer, int? rating)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    NullProperties(sellOffer, buyOffer);
                    buyOffer = await unitOfWork.BuyOfferRepository.GetById(buyOffer.ID);
                    if(buyOffer.status_id == 3)
                    {
                        return ErrorValue.TransactionAlreadyFinished;
                    }
                    var soldProduct = await GetSoldProduct(sellOffer, buyOffer, unitOfWork);
                    if(soldProduct == null)
                    {
                        return ErrorValue.AmountGreaterThanStock;
                    }
                    sellOffer.product = soldProduct;
                    UpdateOffers(sellOffer, buyOffer, unitOfWork);
                    await _ratingUpdater.UpdateRating(buyOffer.buyer_id, unitOfWork, rating);

                    var transaction = new transactions()
                    {
                        buyer_id = buyOffer.buyer_id,
                        seller_id = sellOffer.seller_id,
                        sell_Offer = sellOffer,
                        buy_offer_id = buyOffer.ID,
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
                catch (Exception)
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
            boughtProduct.stock += buyOffer.amount;
            unitOfWork.ProductRepository.Update(boughtProduct);
            var soldProduct = await unitOfWork.ProductRepository.GetData(p => p.product_owner == sellOffer.seller_id
                                                                    && p.Name == boughtProduct.Name);
            if (soldProduct.Count() != 0)
            {
                var p = soldProduct.First();
                if(p.stock < buyOffer.amount)
                {
                    return null;
                }
                p.stock -= buyOffer.amount;
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
