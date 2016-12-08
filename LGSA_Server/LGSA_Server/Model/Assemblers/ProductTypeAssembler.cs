using LGSA_Server.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.Assemblers
{
    public class ProductTypeAssembler : IAssembler<dic_Product_type, ProductTypeDto>
    {
        public IEnumerable<ProductTypeDto> EntityToDto(IEnumerable<dic_Product_type> entity)
        {
            return entity.Select(e => EntityToDto(e));
        }

        public ProductTypeDto EntityToDto(dic_Product_type entity)
        {
            if (entity == null)
            {
                return null;
            }
            return new ProductTypeDto()
            {
                Id = entity.ID,
                Name = entity.name,
            };
        }
    }
}