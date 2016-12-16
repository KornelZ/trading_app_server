using LGSA_Server.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.Assemblers
{
    public class UserAssembler : ITwoWayAssembler<users, UserDto>
    {
        private ITwoWayAssembler<UserAddress, AddressDto> _assembler;

        public UserAssembler(ITwoWayAssembler<UserAddress, AddressDto> assembler)
        {
            _assembler = assembler;
        }
        public IEnumerable<users> DtoToEntity(IEnumerable<UserDto> dto)
        {
            return dto.Select(u => DtoToEntity(u));
        }

        public users DtoToEntity(UserDto dto)
        {
            if(dto == null)
            {
                return null;
            }
            return new users()
            {
                ID = dto.Id,
                First_Name = dto.FirstName,
                Last_Name = dto.LastName,
                UserName = dto.UserName,
                Rating = dto.Rating,
                UserAddress1 = _assembler.DtoToEntity(dto.Address),
                Update_Date = DateTime.Now,
                Update_Who = 1,
                Address_ID = dto.AddressId
            };
        }

        public IEnumerable<UserDto> EntityToDto(IEnumerable<users> entity)
        {
            return entity.Select(e => EntityToDto(e));
        }

        public UserDto EntityToDto(users entity)
        {
            if(entity == null)
            {
                return null;
            }
            return new UserDto()
            {
                Id = entity.ID,
                FirstName = entity.First_Name,
                LastName = entity.Last_Name,
                UserName = entity.UserName,
                Rating = entity.Rating,
                Address = _assembler.EntityToDto(entity.UserAddress1),
                AddressId = entity.Address_ID
            };
        }
    }
}