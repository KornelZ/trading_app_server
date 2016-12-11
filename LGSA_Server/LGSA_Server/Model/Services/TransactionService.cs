using LGSA.Model.UnitOfWork;
using LGSA_Server.Model;
using LGSA_Server.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LGSA.Model.Services
{

    public interface ITransactionService : IDataService<transactions>
    {
        Task<bool> AcceptSellTransaction(sell_Offer sellOffer, buy_Offer buyOffer, int? rating);
        Task<bool> AcceptBuyTransaction(sell_Offer sellOffer, buy_Offer buyOffer, int? rating);

    }
    public class TransactionService : ITransactionService
    {
        private IUnitOfWorkFactory _factory;
        public TransactionService(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }
        public async Task<bool> AcceptSellTransaction(sell_Offer sellOffer, buy_Offer buyOffer, int? rating)
        {
            bool success = true;
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    var boughtProduct = await GetBoughtProduct(sellOffer, buyOffer, unitOfWork);
                    buyOffer.product_id = boughtProduct.ID;
                    UpdateOffers(sellOffer, buyOffer, unitOfWork);

                    var transaction = new transactions()
                    {
                        buyer_id = buyOffer.buyer_id,
                        seller_id = sellOffer.seller_id,
                        buy_offer_id = buyOffer.ID,
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
                catch (Exception e)
                {
                    unitOfWork.Rollback();
                    success = false;
                }
            }
            return success;
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
        private async Task UpdateUserRating(int userId, IUnitOfWork unitOfWork)
        {
            var transactions = await unitOfWork.TransactionRepository.GetData(t => t.Update_Who != userId &&
                (t.buyer_id == userId || t.seller_id == userId));

            int count = transactions.Count(t => t.Rating != null);
            int sum = transactions.Sum(t => t.Rating ?? 0);
            int? rating;
            if(count != 0)
            {
                rating = sum / count;
            }
            else
            {
                rating = null;
            }
            //get user from repository and update rating
        }
        private void UpdateOffers(sell_Offer sellOffer, buy_Offer buyOffer, IUnitOfWork unitOfWork)
        {
            buyOffer.status_id = (int)TransactionState.Finished;
            sellOffer.status_id = (int)TransactionState.Finished;
            if (sellOffer.ID == 0)
            {
                sellOffer = unitOfWork.SellOfferRepository.Add(sellOffer);
                unitOfWork.BuyOfferRepository.Update(buyOffer);
            }
            else if(buyOffer.ID == 0)
            {
                buyOffer = unitOfWork.BuyOfferRepository.Add(buyOffer);
                unitOfWork.SellOfferRepository.Update(sellOffer);
            }
        }
        public async Task<bool> AcceptBuyTransaction(sell_Offer sellOffer, buy_Offer buyOffer, int? rating)
        {
            bool success = true;
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    var soldProduct = await GetSoldProduct(sellOffer, buyOffer, unitOfWork);
                    if(soldProduct == null)
                    {
                        return false;
                    }
                    sellOffer.product_id = soldProduct.ID;
                    UpdateOffers(sellOffer, buyOffer, unitOfWork);

                    var transaction = new transactions()
                    {
                        buyer_id = buyOffer.buyer_id,
                        seller_id = sellOffer.seller_id,
                        buy_offer_id = buyOffer.ID,
                        sell_offer_id = sellOffer.ID,
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
                catch (Exception e)
                {
                    unitOfWork.Rollback();
                    success = false;
                }
            }
            return success;
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
        public async Task<bool> Add(transactions entity)
        {
            bool success = true;
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    unitOfWork.TransactionRepository.Add(entity);
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch (Exception e)
                {
                    unitOfWork.Rollback();
                    success = false;
                }
            }
            return success;
        }
        public async Task<bool> Delete(transactions entity)
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
                catch (Exception e)
                {
                    unitOfWork.Rollback();
                    success = false;
                }
            }
            return success;
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
        public async Task<bool> Update(transactions entity)
        {
            bool success = true;
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    unitOfWork.StartTransaction();
                    unitOfWork.TransactionRepository.Update(entity);
                    await unitOfWork.Save();
                    unitOfWork.Commit();
                }
                catch (Exception e)
                {
                    unitOfWork.Rollback();
                    success = false;
                }
            }
            return success;
        }
    }
}
