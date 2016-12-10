using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace LGSA_Server.Model.DTO
{
    public class FilterDto
    {
        public string ProductName { get; set; }
        public int GenreId { get; set; }
        public int ConditionId { get; set; }
        public int ProductTypeId { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }

        public Expression<Func<product, bool>> GetProductFilter(int userId)
        {

            return null;
        }
        public Expression<Func<buy_Offer, bool>> GetBuyOfferFilter(int userId)
        {
            return null;
        }

        public Expression<Func<sell_Offer, bool>> GetSellOfferFilter(int userId)
        {
            return null;
        }
    }
}