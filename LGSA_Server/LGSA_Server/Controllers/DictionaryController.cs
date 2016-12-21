using LGSA.Model.Services;
using LGSA.Model.UnitOfWork;
using LGSA_Server.Authentication;
using LGSA_Server.Model;
using LGSA_Server.Model.Assemblers;
using LGSA_Server.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace LGSA_Server.Controllers
{
    [Authentication.Authentication]
    public class DictionaryController : ApiController
    {
        private IDictionaryService<dic_condition> _conditionService;
        private IDictionaryService<dic_Genre> _genreService;
        private IDictionaryService<dic_Product_type> _productTypeService;

        private IAssembler<dic_condition, ConditionDto> _conditionAssembler;
        private IAssembler<dic_Genre, GenreDto> _genreAssembler;
        private IAssembler<dic_Product_type, ProductTypeDto> _productTypeAssembler;
        public DictionaryController(IUnitOfWorkFactory factory)
        {
            _conditionService = new ConditionService(factory);
            _genreService = new GenreService(factory);
            _productTypeService = new ProductTypeService(factory);

            _conditionAssembler = new ConditionAssembler();
            _genreAssembler = new GenreAssembler();
            _productTypeAssembler = new ProductTypeAssembler();
        }

        [HttpGet, HttpsOnly]
        public async Task<IHttpActionResult> Get()
        {
            var conditions = _conditionAssembler.EntityToDto(await _conditionService.GetData(null));
            var genres = _genreAssembler.EntityToDto(await _genreService.GetData(null));
            var productTypes = _productTypeAssembler.EntityToDto(await _productTypeService.GetData(null));

            var dto = new DictionaryDto()
            {
                Conditions = conditions,
                Genres = genres,
                ProductTypes = productTypes
            };

            return Ok(dto);
        }
    }
}