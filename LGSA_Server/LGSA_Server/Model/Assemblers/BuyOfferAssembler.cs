using LGSA_Server.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.Assemblers
{
    public class BuyOfferAssembler : ITwoWayAssembler<buy_Offer, BuyOfferDto>
    {
        private ITwoWayAssembler<product, ProductDto> _assembler;
        private ITwoWayAssembler<users, UserDto> _userAssembler;
        public BuyOfferAssembler(ITwoWayAssembler<product, ProductDto> assembler, ITwoWayAssembler<users, UserDto> userAssembler)
        {
            _assembler = assembler;
            _userAssembler = userAssembler;
        }
        public IEnumerable<buy_Offer> DtoToEntity(IEnumerable<BuyOfferDto> dto)
        {
            return dto.Select(b => DtoToEntity(b));
        }

        public buy_Offer DtoToEntity(BuyOfferDto dto)
        {
            return new buy_Offer()
            {
                amount = dto.Amount,
                buyer_id = dto.BuyerId,
                ID = dto.Id,
                name = dto.Name,
                status_id = 1,
                product_id = dto.ProductId,
                price = (double?)dto.Price,
                sold_copies = 0,
                Update_Who = dto.BuyerId,
                Update_Date = DateTime.Now,
                product = _assembler.DtoToEntity(dto.Product)
            };
        }

        public IEnumerable<BuyOfferDto> EntityToDto(IEnumerable<buy_Offer> entity)
        {
            return entity.Select(e => EntityToDto(e));
        }

        public BuyOfferDto EntityToDto(buy_Offer entity)
        {
            return new BuyOfferDto()
            {
                Id = entity.ID,
                Amount = entity.amount,
                BuyerId = entity.buyer_id,
                Name = entity.name,
                Price = (decimal?)entity.price,
                ProductId = entity.product_id,
                Product = _assembler.EntityToDto(entity.product),
                User = _userAssembler.EntityToDto(entity.users)
            };
        }
    }
}