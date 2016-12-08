using LGSA_Server.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.Assemblers
{
    public class GenreAssembler : IAssembler<dic_Genre, GenreDto>
    {
        public IEnumerable<GenreDto> EntityToDto(IEnumerable<dic_Genre> entity)
        {
            return entity.Select(g => EntityToDto(g));
        }

        public GenreDto EntityToDto(dic_Genre entity)
        {
            if (entity == null)
            {
                return null;
            }
            return new GenreDto()
            {
                Id = entity.ID,
                Name = entity.name,
                Description = entity.genre_description
            };
        }
    }
}