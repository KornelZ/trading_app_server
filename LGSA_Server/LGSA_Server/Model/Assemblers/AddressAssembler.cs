using LGSA_Server.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.Assemblers
{
    public class AddressAssembler : ITwoWayAssembler<UserAddress, AddressDto>
    {
        public IEnumerable<UserAddress> DtoToEntity(IEnumerable<AddressDto> dto)
        {
            return dto.Select(a => DtoToEntity(a));
        }

        public UserAddress DtoToEntity(AddressDto dto)
        {
            if(dto == null)
            {
                return null;
            }
            return new UserAddress()
            {
                ID = dto.Id,
                City = dto.City,
                Postal_Code = dto.PostalCode,
                Street = dto.Street,
                Update_Who = 1
            };
        }

        public IEnumerable<AddressDto> EntityToDto(IEnumerable<UserAddress> entity)
        {
            return entity.Select(e => EntityToDto(e));
        }

        public AddressDto EntityToDto(UserAddress entity)
        {
            if(entity == null)
            {
                return null;
            }
            return new AddressDto()
            {
                Id = entity.ID,
                City = entity.City,
                PostalCode = entity.Postal_Code,
                Street = entity.Street,
            };
        }
    }
}