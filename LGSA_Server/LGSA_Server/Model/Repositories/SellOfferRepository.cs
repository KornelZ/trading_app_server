using LGSA_Server.Model;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LGSA.Model.Repositories
{
    public class SellOfferRepository : IRepository<sell_Offer>
    {
        protected DbContext _context;
        public SellOfferRepository(DbContext context)
        {
            _context = context;
        }        
        public virtual sell_Offer Add(sell_Offer entity)
        {
            Attach(_context, entity);
            _context.Set<sell_Offer>().Include(b => b.product);
            
            return _context.Set<sell_Offer>().Add(entity); ;
        }
        public virtual bool Update(sell_Offer entity)
        {
            Attach(_context, entity);
            _context.Entry(entity).State = EntityState.Modified;
            return true;
        }
        public virtual async Task<IEnumerable<sell_Offer>> GetData(Expression<Func<sell_Offer, bool>> filter)
        {
            return await _context.Set<sell_Offer>()
                .Include(sell_Offer => sell_Offer.users)
                .Include(sell_Offer => sell_Offer.product)
                .Include(sell_Offer => sell_Offer.dic_Offer_status)
                .Include(sell_Offer => sell_Offer.product.dic_condition)
                .Include(sell_Offer => sell_Offer.product.dic_Product_type)
                .Include(sell_Offer => sell_Offer.product.dic_Genre)
                .AsExpandable()
                .Where(filter).ToListAsync();
        }

        public virtual async Task<sell_Offer> GetById(int id)
        {
            return await _context.Set<sell_Offer>()
                .Include(sell_Offer => sell_Offer.users)
                .Include(sell_Offer => sell_Offer.product)
                .Include(sell_Offer => sell_Offer.product.dic_Genre)
                .Include(sell_Offer => sell_Offer.product.dic_condition)
                .Include(sell_Offer => sell_Offer.product.dic_Product_type)
                .Include(sell_Offer => sell_Offer.dic_Offer_status)
                .Include(sell_Offer => sell_Offer.users)
                .Include(sell_Offer => sell_Offer.users.UserAddress)
                .FirstOrDefaultAsync(b => b.ID == id);
        }

        public static void Attach(DbContext ctx, sell_Offer entity)
        {
            if (entity.users != null)
            {
                ctx.Set<users>().Attach(entity.users);
            }
            if (entity.dic_Offer_status != null)
            {
                ctx.Set<dic_Offer_status>().Attach(entity.dic_Offer_status);
            }
            if (entity.product != null)
            {
                ctx.Set<product>().Attach(entity.product);
                ProductRepository.Attach(ctx, entity.product);
            }
        }

        public virtual bool Delete(sell_Offer entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            return true;
        }
    }
}
