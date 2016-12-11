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
    public class BuyOfferController : ApiController
    {
        private ITwoWayAssembler<buy_Offer, BuyOfferDto> _assembler;
        private IDataService<buy_Offer> _service;
        public BuyOfferController(IUnitOfWorkFactory factory)
        {
            _service = new BuyOfferService(factory);

            _assembler = new BuyOfferAssembler(new ProductAssembler(new ConditionAssembler(),
                                                                    new GenreAssembler(),
                                                                    new ProductTypeAssembler()));
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] BuyOfferFilterDto filter)
        {
            var id = (Thread.CurrentPrincipal as UserPrincipal).Id;
            var offers = await _service.GetData(filter.GetFilter(id));
            var dto = _assembler.EntityToDto(offers);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] BuyOfferDto dto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var offer = _assembler.DtoToEntity(dto);
            var result = await _service.Add(offer);
            if(result == false)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody] BuyOfferDto dto)
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
        public async Task<IHttpActionResult> Delete([FromBody] BuyOfferDto dto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var offer = _assembler.DtoToEntity(dto);

            var result = await _service.Delete(offer);

            if(result == false)
            {
                return NotFound();
            }

            return Ok();
        }

    }
}