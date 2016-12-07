using LGSA.Model.UnitOfWork;
using LGSA_Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LGSA.Model.Services
{
    public class GenreService : IDictionaryService<dic_Genre>
    {
        private IUnitOfWorkFactory _factory;
        public GenreService(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }
        public async Task<IEnumerable<dic_Genre>> GetData(Expression<Func<dic_Genre, bool>> filter)
        {
            using (var unitOfWork = _factory.CreateUnitOfWork())
            {
                try
                {
                    var entities = await unitOfWork.GenreRepository.GetData(filter);

                    return entities;
                }
                catch (Exception e)
                {
                }
            }

            return null;
        }
    }
}
