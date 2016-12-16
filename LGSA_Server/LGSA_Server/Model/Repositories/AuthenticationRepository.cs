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
    public class AuthenticationRepository : Repository<users_Authetication>
    {
        public AuthenticationRepository(DbContext context) : base(context)
        {
        }
        public override bool Update(users_Authetication entity)
        {
            Attach(_context, entity);
            return base.Update(entity);
        }
        public override Task<users_Authetication> GetById(int id)
        {

            return _context.Set<users_Authetication>()
                .Include(users_Authetication => users_Authetication.users1)
                .Include(users_Authetication => users_Authetication.users1.UserAddress1)
                .Where(u => u.User_id == id).SingleOrDefaultAsync();
        }
        public override users_Authetication Add(users_Authetication entity)
        {
            if(_context.Set<users_Authetication>()
                .Include(users_Authetication => users_Authetication.users1)
                .Include(users_Authetication => users_Authetication.users1.UserAddress1)
                .Any(u => u.users1.First_Name == entity.users1.First_Name &&
                u.users1.Last_Name == entity.users1.Last_Name))
            {
                return null;
            }
            Attach(_context, entity);
            return base.Add(entity);
        }
        public override async Task<IEnumerable<users_Authetication>> GetData(Expression<Func<users_Authetication, bool>> filter)
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
    }
}
