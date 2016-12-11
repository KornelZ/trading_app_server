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
        private ITwoWayAssembler<buy_Offer, BuyOfferDto> _buyAssembler;
        private ITwoWayAssembler<sell_Offer, SellOfferDto> _sellAssembler;
        private IDataService<buy_Offer> _service;
        private ITransactionService _transactionService;
        public BuyOfferController(IUnitOfWorkFactory factory)
        {
            _service = new BuyOfferService(factory);

            _buyAssembler = new BuyOfferAssembler(new ProductAssembler(new ConditionAssembler(),
                                                                    new GenreAssembler(),
                                                                    new ProductTypeAssembler()));
            _sellAssembler = new SellOfferAssembler(new ProductAssembler(new ConditionAssembler(),
                                                                    new GenreAssembler(),
                                                                    new ProductTypeAssembler()));
            _transactionService = new TransactionService(factory);
        }

        [HttpPost, Route("AcceptBuyTransaction/")]
        public async Task<IHttpActionResult> AcceptBuyTransaction([FromBody] TransactionDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var sellOffer = _sellAssembler.DtoToEntity(dto.SellOffer);
            var buyOffer = _buyAssembler.DtoToEntity(dto.BuyOffer);
            var rating = dto.Rating;

            var result = await _transactionService.AcceptBuyTransaction(sellOffer, buyOffer, rating);

            if (result == false)
            {
                return BadRequest("Error occured during the transaction");
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] BuyOfferFilterDto filter)
        {
            var id = (Thread.CurrentPrincipal as UserPrincipal).Id;
            var offers = await _service.GetData(filter.GetFilter(id));
            var dto = _buyAssembler.EntityToDto(offers);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] BuyOfferDto dto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var offer = _buyAssembler.DtoToEntity(dto);
            var result = await _service.Add(offer);
            if(result == false)
            {
                return BadRequest(ModelState);
            }
            dto = _buyAssembler.EntityToDto(offer);
            return Ok(dto);
        }

        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody] BuyOfferDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var offer = _buyAssembler.DtoToEntity(dto);

            var result = await _service.Update(offer);

            if (result == false)
            {
                return NotFound();
            }
            dto = _buyAssembler.EntityToDto(offer);
            return Ok(dto);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromBody] BuyOfferDto dto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var offer = _buyAssembler.DtoToEntity(dto);

            var result = await _service.Delete(offer);

            if(result == false)
            {
                return NotFound();
            }

            return Ok();
        }

    }
}