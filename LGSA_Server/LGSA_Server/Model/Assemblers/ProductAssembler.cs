using LGSA_Server.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.Assemblers
{
    public class ProductAssembler : ITwoWayAssembler<product, ProductDto>
    {
        private IAssembler<dic_condition, ConditionDto> _conditionAssembler;
        private IAssembler<dic_Genre, GenreDto> _genreAssembler;
        private IAssembler<dic_Product_type, ProductTypeDto> _productTypeAssembler;
        public ProductAssembler(IAssembler<dic_condition, ConditionDto> conditionAssembler,
                                IAssembler<dic_Genre, GenreDto> genreAssembler,
                                IAssembler<dic_Product_type, ProductTypeDto> productTypeAssembler)
        {
            _conditionAssembler = conditionAssembler;
            _genreAssembler = genreAssembler;
            _productTypeAssembler = productTypeAssembler;
        }
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
                Update_Date = DateTime.Now,
                Update_Who = dto.ProductOwner,
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
            if (entity == null)
            {
                return null;
            }
            return new ProductDto()
            {
                Id = entity.ID,
                Name = entity.Name,
                ProductOwner = entity.product_owner,
                SoldCopies = entity.sold_copies,
                Stock = entity.stock,
                ConditionId = entity.condition_id,
                GenreId = entity.genre_id,
                ProductTypeId = entity.product_type_id,
                Condition = _conditionAssembler.EntityToDto(entity.dic_condition),
                Genre = _genreAssembler.EntityToDto(entity.dic_Genre),
                ProductType = _productTypeAssembler.EntityToDto(entity.dic_Product_type)
            };
        }
    }
}