using LGSA_Server.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.Assemblers
{
    public class ProductAssembler : IAssembler<product, ProductDto>
    {
        public IEnumerable<product> DtoToEntity(IEnumerable<ProductDto> dto)
        {
            return dto.Select(p => DtoToEntity(p));
        }

        public product DtoToEntity(ProductDto dto)
        {
            return new product()
            {
                ID = dto.Id,
                Name = dto.Name,
                product_owner = dto.ProductOwner,
                sold_copies = dto.SoldCopies,
                stock = dto.Stock,
                Update_Date = dto.UpdateDate,
                Update_Who = dto.UpdateWho,
                condition_id = dto.ConditionId,
                genre_id = dto.GenreId,
                product_type_id = dto.ProductTypeId
            };
        }

        public IEnumerable<ProductDto> EntityToDto(IEnumerable<product> entity)
        {
            return entity.Select(p => EntityToDto(p));
        }

        public ProductDto EntityToDto(product entity)
        {
            return new ProductDto()
            {
                Id = entity.ID,
                Name = entity.Name,
                ProductOwner = entity.product_owner,
                SoldCopies = entity.sold_copies,
                Stock = entity.stock,
                UpdateDate = entity.Update_Date,
                UpdateWho = entity.Update_Who,
                ConditionId = entity.condition_id,
                GenreId = entity.genre_id,
                ProductTypeId = entity.product_type_id
            };
        }
    }
}