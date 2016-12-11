using LGSA_Server.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.Assemblers
{
    public class SellOfferAssembler : ITwoWayAssembler<sell_Offer, SellOfferDto>
    {
        private ITwoWayAssembler<product, ProductDto> _assembler;
        public SellOfferAssembler(ITwoWayAssembler<product, ProductDto> assembler)
        {
            _assembler = assembler;
        }
        public IEnumerable<sell_Offer> DtoToEntity(IEnumerable<SellOfferDto> dto)
        {
            return dto.Select(b => DtoToEntity(b));
        }

        public sell_Offer DtoToEntity(SellOfferDto dto)
        {
            return new sell_Offer()
            {
                amount = dto.Amount,
                seller_id = dto.SellerId,
                ID = dto.Id,
                name = dto.Name,
                status_id = 1,
                product_id = dto.ProductId,
                price = (double?)dto.Price,
                buyed_copies = 0,
                Update_Who = dto.SellerId,
                Update_Date = DateTime.Now,
                product = _assembler.DtoToEntity(dto.Product)
            };
        }

        public IEnumerable<SellOfferDto> EntityToDto(IEnumerable<sell_Offer> entity)
        {
            return entity.Select(e => EntityToDto(e));
        }

        public SellOfferDto EntityToDto(sell_Offer entity)
        {
            return new SellOfferDto()
            {
                Id = entity.ID,
                Amount = entity.amount,
                SellerId = entity.seller_id,
                Name = entity.name,
                Price = (decimal?)entity.price,
                ProductId = entity.product_id,
                Product = _assembler.EntityToDto(entity.product)
            };
        }
    }
}