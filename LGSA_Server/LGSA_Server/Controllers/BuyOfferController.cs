using LGSA.Model.Services;
using LGSA.Model.UnitOfWork;
using LGSA_Server.Authentication;
using LGSA_Server.Model;
using LGSA_Server.Model.Assemblers;
using LGSA_Server.Model.DTO;
using LGSA_Server.Model.DTO.Filters;
using LGSA_Server.Model.Enums;
using LGSA_Server.Model.Services.TransactionLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace LGSA_Server.Controllers
{
    public class BuyOfferController : ApiController
    {
        private ITwoWayAssembler<buy_Offer, BuyOfferDto> _buyAssembler;
        private ITwoWayAssembler<sell_Offer, SellOfferDto> _sellAssembler;
        private IDataService<buy_Offer> _service;
        private ITransactionService _transactionService;
        public BuyOfferController(IUnitOfWorkFactory factory, IRatingUpdater ratingUpdater)
        {
            _service = new BuyOfferService(factory);

            _buyAssembler = new BuyOfferAssembler(new ProductAssembler(new ConditionAssembler(),
                                                                    new GenreAssembler(),
                                                                    new ProductTypeAssembler()),
                                                  new UserAssembler(new AddressAssembler()));
            _sellAssembler = new SellOfferAssembler(new ProductAssembler(new ConditionAssembler(),
                                                                    new GenreAssembler(),
                                                                    new ProductTypeAssembler()),
                                                    new UserAssembler(new AddressAssembler()));
            _transactionService = new TransactionService(factory, ratingUpdater);
        }
        [Authentication.Authentication]
        [HttpPost, Route("AcceptBuyTransaction/")]
        public async Task<IHttpActionResult> AcceptBuyTransaction([FromBody] TransactionDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest("Invalid data");
            }
            if (dto.SellOffer.SellerId != (Thread.CurrentPrincipal as UserPrincipal).Id)
            {
                return Unauthorized();
            }

            var sellOffer = _sellAssembler.DtoToEntity(dto.SellOffer);
            var buyOffer = _buyAssembler.DtoToEntity(dto.BuyOffer);
            var rating = dto.Rating;

            var result = await _transactionService.AcceptBuyTransaction(sellOffer, buyOffer, rating);

            if (result == ErrorValue.AmountGreaterThanStock)
            {
                return BadRequest("Amount greater than stock");
            }
            else if (result == ErrorValue.ServerError)
            {
                return BadRequest("Transaction error");
            }
            return Ok();
        }
        [Authentication.Authentication]
        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] BuyOfferFilterDto filter)
        {
            var id = (Thread.CurrentPrincipal as UserPrincipal).Id;
            var offers = await _service.GetData(filter.GetFilter(id));
            var dto = _buyAssembler.EntityToDto(offers);

            return Ok(dto);
        }
        [Authentication.Authentication]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] BuyOfferDto dto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest("Invalid data");
            }
            if (dto.BuyerId != (Thread.CurrentPrincipal as UserPrincipal).Id)
            {
                return Unauthorized();
            }

            var offer = _buyAssembler.DtoToEntity(dto);
            var result = await _service.Add(offer);
            if(result == ErrorValue.ServerError)
            {
                return BadRequest("Transaction error");
            }
            dto = _buyAssembler.EntityToDto(offer);
            return Ok(dto);
        }
        [Authentication.Authentication]
        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody] BuyOfferDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest("Invalid data");
            }
            if (dto.BuyerId != (Thread.CurrentPrincipal as UserPrincipal).Id)
            {
                return Unauthorized();
            }

            var offer = _buyAssembler.DtoToEntity(dto);

            var result = await _service.Update(offer);

            if (result == ErrorValue.ServerError)
            {
                return BadRequest("Entity not found");
            }
            dto = _buyAssembler.EntityToDto(offer);
            return Ok(dto);
        }
        [Authentication.Authentication]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromBody] BuyOfferDto dto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest("Invalid data");
            }
            if (dto.BuyerId != (Thread.CurrentPrincipal as UserPrincipal).Id)
            {
                return Unauthorized();
            }

            var offer = _buyAssembler.DtoToEntity(dto);

            var result = await _service.Delete(offer);

            if(result == ErrorValue.ServerError)
            {
                return BadRequest("Entity not found");
            }

            return Ok();
        }

    }
}