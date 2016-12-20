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
    public class ProductRepository : IRepository<product>
    {
        protected DbContext _context;
        public ProductRepository(DbContext context)
        {
            _context = context;
        }
        public virtual product Add(product entity)
        {
            Attach(_context, entity);
           
            return _context.Set<product>().Add(entity);
        }

        public virtual bool Update(product entity)
        {
            _context.Set<product>().Attach(entity);
            Attach(_context, entity);
            _context.Entry(entity).State = EntityState.Modified;
            return true;
        }
        public virtual async Task<IEnumerable<product>> GetData(Expression<Func<product, bool>> filter)
        {

            var products = _context.Set<product>()
                .Include(product => product.users)
                .Include(product => product.dic_condition)
                .Include(product => product.dic_Genre)
                .Include(product => product.dic_Product_type);

            if(filter != null)
            {
                products = products.AsExpandable().Where(filter);
            }

            return await products.ToListAsync();
        }

        public static void Attach(DbContext ctx, product entity)
        {
            if(entity.users != null)
            {
                ctx.Entry(entity.users).State = EntityState.Modified;
            }
            if(entity.dic_condition != null)
            {
                ctx.Entry(entity.dic_condition).State = EntityState.Modified;
            }
            if(entity.dic_Genre != null)
            {
                ctx.Entry(entity.dic_Genre).State = EntityState.Modified;
            }
            if(entity.dic_Product_type != null)
            {
                ctx.Entry(entity.dic_Product_type).State = EntityState.Modified;
            }
        }

        public async Task<product> GetById(int id)
        {
            return await _context.Set<product>().FindAsync(id);
        }

        public bool Delete(product entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            return true;
        }
    }
}
