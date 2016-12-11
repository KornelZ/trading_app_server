using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace LGSA_Server.Model.DTO.Filters
{
    public class ProductFilterDto : IFilterDto<product>
    {
        public string ProductName { get; set; }
        public int? GenreId { get; set; }
        public int? ConditionId { get; set; }
        public int? ProductTypeId { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }

        public Expression<Func<product, bool>> GetFilter(int userId)
        {
            Expression<Func<product, bool>> filter = p => p.product_owner == userId && p.stock >= Stock;

            if(ProductName != null)
            {
                Expression<Func<product, bool>> f = p => p.Name.Contains(ProductName);
                filter = Expression.Lambda<Func<product, bool>>(Expression.And(filter.Body, f.Body), filter.Parameters[0]);
            }
            if(ConditionId != null)
            {
                Expression<Func<product, bool>> f = p => p.condition_id == ConditionId;
                filter = Expression.Lambda<Func<product, bool>>(Expression.And(filter.Body, f.Body), filter.Parameters[0]);
            }
            if(GenreId != null)
            {
                Expression<Func<product, bool>> f = p => p.genre_id == GenreId;
                filter = Expression.Lambda<Func<product, bool>>(Expression.And(filter.Body, f.Body), filter.Parameters[0]);
            }
            if(ProductTypeId != null)
            {
                Expression<Func<product, bool>> f = p => p.product_type_id == ProductTypeId;
                filter = Expression.Lambda<Func<product, bool>>(Expression.And(filter.Body, f.Body), filter.Parameters[0]);
            }

            return filter;
        }
    }
}