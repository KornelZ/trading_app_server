using LGSA.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LGSA_Server.Model.Services.TransactionLogic
{
    public interface IRatingUpdater
    {
        Task UpdateRating(int id, IUnitOfWork unitOfWork, int? rating);
    }
    public class RatingUpdater : IRatingUpdater
    {
        public async Task UpdateRating(int id, IUnitOfWork unitOfWork, int? rating)
        {
            if (rating != null)
            {
                var transactions = await GetTransactions(id, unitOfWork);

                int count = transactions.Count(t => t.Rating != null) + 1;
                int sum = transactions.Sum(t => t.Rating ?? 0) + (int)rating;

                if(sum != 0)
                {
                    await UpdateUser(id, unitOfWork, sum / count);
                }
            }
        }

        private async Task<IEnumerable<transactions>> GetTransactions(int id, IUnitOfWork unitOfWork)
        {
            return await unitOfWork.TransactionRepository.GetData(t => t.Update_Who != id &&
                    (t.buyer_id == id || t.seller_id == id));
        }

        private async Task UpdateUser(int id, IUnitOfWork unitOfWork, int? rating)
        {
            var result = await unitOfWork.AuthenticationRepository.GetById(id);
            result.users1.Rating = rating;
        }
    }
}