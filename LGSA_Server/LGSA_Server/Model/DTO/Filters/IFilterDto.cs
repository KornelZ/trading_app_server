using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LGSA_Server.Model.DTO.Filters
{
    public interface IFilterDto<T>
    {
        string ProductName { get; set; }
        int? GenreId { get; set; }
        int? ConditionId { get; set; }
        int? ProductTypeId { get; set; }
        double Rating { get; set; }
        int Stock { get; set; }
        Expression<Func<T, bool>> GetFilter(int userId);
    }
}
