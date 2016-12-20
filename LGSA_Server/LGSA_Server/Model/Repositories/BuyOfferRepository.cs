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
    public class BuyOfferRepository : IRepository<buy_Offer>
    {
        protected DbContext _context;
        public BuyOfferRepository(DbContext context)
        {
            _context = context;
        }

        public virtual buy_Offer Add(buy_Offer entity)
        {
            Attach(_context, entity);
            _context.Set<buy_Offer>().Include(b => b.product);
            return _context.Set<buy_Offer>().Add(entity);
        }

        public virtual bool Update(buy_Offer entity)
        {
            Attach(_context, entity);
            _context.Entry(entity).State = EntityState.Modified;
            return true;
        }

        public virtual async Task<IEnumerable<buy_Offer>> GetData(Expression<Func<buy_Offer, bool>> filter)
        {
            return await _context.Set<buy_Offer>()
                .Include(buy_Offer => buy_Offer.users)
                .Include(buy_Offer => buy_Offer.product)
                .Include(buy_Offer => buy_Offer.dic_Offer_status)
                .Include(buy_Offer => buy_Offer.product.dic_condition)
                .Include(buy_Offer => buy_Offer.product.dic_Product_type)
                .Include(buy_Offer => buy_Offer.product.dic_Genre)
                .Include(buy_Offer => buy_Offer.users)
                .Include(buy_Offer => buy_Offer.users.UserAddress1)
                .AsExpandable()
                .Where(filter).ToListAsync();
        }

        public virtual async Task<buy_Offer> GetById(int id)
        {
            return await _context.Set<buy_Offer>()
                .Include(buy_Offer => buy_Offer.users)
                .Include(buy_Offer => buy_Offer.product)
                .Include(buy_Offer => buy_Offer.product.dic_Genre)
                .Include(buy_Offer => buy_Offer.product.dic_condition)
                .Include(buy_Offer => buy_Offer.product.dic_Product_type)
                .Include(buy_Offer => buy_Offer.dic_Offer_status)
                .FirstOrDefaultAsync(b => b.ID == id);
        }

        public static void Attach(DbContext ctx, buy_Offer entity)
        {
            if(entity.users != null)
            {
                ctx.Entry(entity.users).State = EntityState.Modified;
            }
            if(entity.dic_Offer_status != null)
            {
                ctx.Entry(entity.dic_Offer_status).State = EntityState.Modified;
            }
            if(entity.product != null)
            {
                if(entity.product.ID != 0)
                {
                    ctx.Entry(entity.product).State = EntityState.Modified;
                }
               
                ProductRepository.Attach(ctx, entity.product);
            }
        }

        public bool Delete(buy_Offer entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            return true;
        }
    }
}
