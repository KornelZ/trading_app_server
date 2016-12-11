using LGSA_Server.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.Assemblers
{
    public class AuthenticationAssembler : ITwoWayAssembler<users_Authetication, AuthenticationDto>
    {
        private ITwoWayAssembler<users, UserDto> _assembler;
        public AuthenticationAssembler(ITwoWayAssembler<users, UserDto> assembler)
        {
            _assembler = assembler;
        }
        public IEnumerable<users_Authetication> DtoToEntity(IEnumerable<AuthenticationDto> dto)
        {
            return dto.Select(u => DtoToEntity(u));
        }

        public users_Authetication DtoToEntity(AuthenticationDto dto)
        {
            return new users_Authetication()
            {
                ID = dto.Id,
                password = dto.Password,
                users1 = _assembler.DtoToEntity(dto.User),
                Update_Date = DateTime.Now,
                Update_Who = 1,
                User_id = dto.UserId
            };
        }

        public IEnumerable<AuthenticationDto> EntityToDto(IEnumerable<users_Authetication> entity)
        {
            return entity.Select(e => EntityToDto(e));
        }

        public AuthenticationDto EntityToDto(users_Authetication entity)
        {
            return new AuthenticationDto()
            {
                Id = entity.ID,
                Password = entity.password,
                User = _assembler.EntityToDto(entity.users1),
                UserId = entity.User_id              
            };
        }
    }
}