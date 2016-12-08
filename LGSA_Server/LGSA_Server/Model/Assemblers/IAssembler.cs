using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LGSA_Server.Model.Assemblers
{

    public interface IAssembler<TEntity, TDto> where TEntity : class
                                               where TDto : class
    {
        TDto EntityToDto(TEntity entity);
        IEnumerable<TDto> EntityToDto(IEnumerable<TEntity> entity);
    }
    public interface ITwoWayAssembler<TEntity, TDto> : IAssembler<TEntity, TDto> where TEntity: class
                                               where TDto: class
    {
        TEntity DtoToEntity(TDto dto);
        IEnumerable<TEntity> DtoToEntity(IEnumerable<TDto> dto);

    }
}