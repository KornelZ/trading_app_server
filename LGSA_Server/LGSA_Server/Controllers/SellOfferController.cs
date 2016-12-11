using LGSA.Model.Services;
using LGSA.Model.UnitOfWork;
using LGSA_Server.Authentication;
using LGSA_Server.Model;
using LGSA_Server.Model.Assemblers;
using LGSA_Server.Model.DTO;
using LGSA_Server.Model.DTO.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace LGSA_Server.Controllers
{
    [Authentication.Authentication]
    public class SellOfferController : ApiController
    {
        private ITwoWayAssembler<sell_Offer, SellOfferDto> _assembler;
        private IDataService<sell_Offer> _service;
        public SellOfferController(IUnitOfWorkFactory factory)
        {
            _service = new SellOfferService(factory);

            _assembler = new SellOfferAssembler(new ProductAssembler(new ConditionAssembler(),
                                                                    new GenreAssembler(),
                                                                    new ProductTypeAssembler()));
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] SellOfferFilterDto filter)
        {
            var id = (Thread.CurrentPrincipal as UserPrincipal).Id;
            var offers = await _service.GetData(filter.GetFilter(id));
            var dto = _assembler.EntityToDto(offers);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] SellOfferDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var offer = _assembler.DtoToEntity(dto);

            var result = await _service.Add(offer);
            if (result == false)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody] SellOfferDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var offer = _assembler.DtoToEntity(dto);

            var result = await _service.Update(offer);

            if (result == false)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromBody] SellOfferDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var offer = _assembler.DtoToEntity(dto);

            var result = await _service.Delete(offer);

            if (result == false)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}