using LGSA_Server.Model.DTO.Filters;
using LinqKit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace LGSA_Server.Model.DTO.Filters
{
    public class BuyOfferFilterDto : IFilterDto<buy_Offer>
    {
        public string ProductName { get; set; }
        public int? GenreId { get; set; }
        public int? ConditionId { get; set; }
        public int? ProductTypeId { get; set; }
        [Range(0, 5)]
        public double Rating { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int Stock { get; set; }
        [Range(0, int.MaxValue)]
        public decimal Price { get; set; }
        [Required]
        public bool ShowMyOffers { get; set; }

        public Expression<Func<buy_Offer, bool>> GetFilter(int userId)
        {
            var builder = PredicateBuilder.New<buy_Offer>();
            builder.And(b => b.product.stock >= Stock && b.status_id == 1);
            if(Rating != 0)
            {
                builder.And(b => b.users.Rating >= Rating || b.users.Rating == null);
            }
            if(ShowMyOffers == true)
            {
                builder.And(b => b.buyer_id == userId);
            }
            else
            {
                builder.And(b => b.buyer_id != userId);
            }
            if (Price != 0)
            {
                builder.And(b => b.price <= (double)Price);
            }
            if (ProductName != null)
            {
                builder.And(b => b.product.Name.Contains(ProductName));
            }
            if (ConditionId != null)
            {
                builder.And(b => b.product.condition_id == ConditionId);
            }
            if (GenreId != null)
            {
                builder.And(b => b.product.genre_id == GenreId);
            }
            if (ProductTypeId != null)
            {
                builder.And(b => b.product.product_type_id == ProductTypeId);
            }

            return builder;
        }
    }
}