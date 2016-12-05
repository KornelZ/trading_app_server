using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using LGSA_Server.Model;
using LGSA.Model.Services;
using LGSA.Model.UnitOfWork;
using System.Threading.Tasks;
using LGSA_Server.Model.DTO;

namespace LGSA_Server.Controllers
{
    public class ProductController : ApiController
    {
        private IService<product> service = new ProductService(new DbUnitOfWorkFactory());

        // GET: api/Product
        public async Task<IEnumerable<ProductDto>> Get()
        {
            var product = await service.GetData(null);
            return product.Select(p =>
                new ProductDto()
                {
                    Id = p.ID,
                    Name = p.Name,
                    ProductOwner = p.product_owner,
                    SoldCopies = p.sold_copies,
                    Stock = p.stock,
                    UpdateDate = p.Update_Date,
                    UpdateWho = p.Update_Who
                });
        }

    }
}