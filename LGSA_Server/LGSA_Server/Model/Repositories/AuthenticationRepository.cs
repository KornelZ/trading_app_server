using LGSA.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using LGSA_Server.Model;
using LinqKit;

namespace LGSA.Model.Repositories
{
    public class AuthenticationRepository : IRepository<users_Authetication>
    {
        protected DbContext _context;
        public AuthenticationRepository(DbContext context)
        {
            _context = context;
        }
        public virtual bool Update(users_Authetication entity)
        {
            Attach(_context, entity);
            _context.Entry(entity).State = EntityState.Modified;
            return true;
        }
        public virtual Task<users_Authetication> GetById(int id)
        {

            return _context.Set<users_Authetication>()
                .Include(users_Authetication => users_Authetication.users1)
                .Include(users_Authetication => users_Authetication.users1.UserAddress1)
                .Where(u => u.User_id == id).SingleOrDefaultAsync();
        }
        public virtual users_Authetication Add(users_Authetication entity)
        {
            if(_context.Set<users_Authetication>()
                .Include(users_Authetication => users_Authetication.users1)
                .Include(users_Authetication => users_Authetication.users1.UserAddress1)
                .Any(u => (u.users1.First_Name == entity.users1.First_Name &&
                u.users1.Last_Name == entity.users1.Last_Name) || u.users1.UserName == entity.users1.UserName))
            {
                return null;
            }
            Attach(_context, entity);
            return _context.Set<users_Authetication>().Add(entity);
        }
        public virtual async Task<IEnumerable<users_Authetication>> GetData(Expression<Func<users_Authetication, bool>> filter)
        {
            if (filter == null)
            {
                return await _context.Set<users_Authetication>()
                    .Include(u => u.users1)
                    .Include(u => u.users1.UserAddress1).ToListAsync();
            }

            return await _context.Set<users_Authetication>()
                .Include(u => u.users1)
                .Include(u => u.users1.UserAddress1).AsExpandable().Where(filter).ToListAsync();
        }
        public static void Attach(DbContext ctx, users_Authetication entity)
        {
            if (entity.users1 != null)
            {
                if(entity.users1.ID != 0)
                {
                    ctx.Entry(entity.users1).State = EntityState.Modified;
                }
                if(entity.users1.UserAddress1 != null)
                {
                    if(entity.users1.UserAddress1.ID != 0)
                    {
                        ctx.Entry(entity.users1.UserAddress1).State = EntityState.Modified;
                    }
                }
            }
        }

        public bool Delete(users_Authetication entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            return true;
        }
    }
}
