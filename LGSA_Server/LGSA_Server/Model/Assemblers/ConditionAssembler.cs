using LGSA_Server.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.Assemblers
{
    public class ConditionAssembler : IAssembler<dic_condition, ConditionDto>
    {
        public IEnumerable<ConditionDto> EntityToDto(IEnumerable<dic_condition> entity)
        {
            return entity.Select(c => EntityToDto(c));
        }

        public ConditionDto EntityToDto(dic_condition entity)
        {
            if(entity == null)
            {
                return null;
            }
            return new ConditionDto()
            {
                Id = entity.ID,
                Name = entity.name
            };
        }
    }
}