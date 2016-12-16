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
        private ITwoWayAssembler<sell_Offer, SellOfferDto> _sellAssembler;
        private ITwoWayAssembler<buy_Offer, BuyOfferDto> _buyAssembler;
        private IDataService<sell_Offer> _service;
        private ITransactionService _transactionService;
        public SellOfferController(IUnitOfWorkFactory factory)
        {
            _service = new SellOfferService(factory);
            _transactionService = new TransactionService(factory);
            _sellAssembler = new SellOfferAssembler(new ProductAssembler(new ConditionAssembler(),
                                                                    new GenreAssembler(),
                                                                    new ProductTypeAssembler()));

            _buyAssembler = new BuyOfferAssembler(new ProductAssembler(new ConditionAssembler(),
                                                                    new GenreAssembler(),
                                                                    new ProductTypeAssembler()));
        }
        [Authentication.Authentication]
        [HttpPost, Route("AcceptSellTransaction/")]
        public async Task<IHttpActionResult> AcceptSellTransaction([FromBody] TransactionDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            if (dto.BuyOffer.BuyerId != (Thread.CurrentPrincipal as UserPrincipal).Id)
            {
                return BadRequest("Internal error");
            }

            var sellOffer = _sellAssembler.DtoToEntity(dto.SellOffer);
            var buyOffer = _buyAssembler.DtoToEntity(dto.BuyOffer);
            var rating = dto.Rating;

            var result = await _transactionService.AcceptSellTransaction(sellOffer, buyOffer, rating);

            if (result == false)
            {
                return BadRequest("Error occured during the transaction");
            }
            return Ok();
        }
        [Authentication.Authentication]
        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] SellOfferFilterDto filter)
        {
            var id = (Thread.CurrentPrincipal as UserPrincipal).Id;
            var offers = await _service.GetData(filter.GetFilter(id));
            var dto = _sellAssembler.EntityToDto(offers);

            return Ok(dto);
        }
        [Authentication.Authentication]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] SellOfferDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            if (dto.SellerId != (Thread.CurrentPrincipal as UserPrincipal).Id)
            {
                return BadRequest("Internal error");
            }

            var offer = _sellAssembler.DtoToEntity(dto);

            var result = await _service.Add(offer);
            if (result == false)
            {
                return BadRequest(ModelState);
            }
            dto = _sellAssembler.EntityToDto(offer);
            return Ok(dto);
        }
        [Authentication.Authentication]
        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody] SellOfferDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            if (dto.SellerId != (Thread.CurrentPrincipal as UserPrincipal).Id)
            {
                return BadRequest("Internal error");
            }

            var offer = _sellAssembler.DtoToEntity(dto);

            var result = await _service.Update(offer);

            if (result == false)
            {
                return NotFound();
            }
            dto = _sellAssembler.EntityToDto(offer);
            return Ok(dto);
        }
        [Authentication.Authentication]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromBody] SellOfferDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            if (dto.SellerId != (Thread.CurrentPrincipal as UserPrincipal).Id)
            {
                return BadRequest("Internal error");
            }

            var offer = _sellAssembler.DtoToEntity(dto);

            var result = await _service.Delete(offer);

            if (result == false)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}