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
    [Authentication.Authentication]
    public class SellOfferController : ApiController
    {
        private ITwoWayAssembler<sell_Offer, SellOfferDto> _sellAssembler;
        private ITwoWayAssembler<buy_Offer, BuyOfferDto> _buyAssembler;
        private IDataService<sell_Offer> _service;
        private ITransactionService _transactionService;
        public SellOfferController(IUnitOfWorkFactory factory, IRatingUpdater ratingUpdater)
        {
            _service = new SellOfferService(factory);
            _transactionService = new TransactionService(factory, ratingUpdater);
            _sellAssembler = new SellOfferAssembler(new ProductAssembler(new ConditionAssembler(),
                                                                    new GenreAssembler(),
                                                                    new ProductTypeAssembler()),
                                                    new UserAssembler(new AddressAssembler()));

            _buyAssembler = new BuyOfferAssembler(new ProductAssembler(new ConditionAssembler(),
                                                                    new GenreAssembler(),
                                                                    new ProductTypeAssembler()),
                                                  new UserAssembler(new AddressAssembler()));
        }
        [Authentication.Authentication, HttpsOnly]
        [HttpPost, Route("AcceptSellTransaction/")]
        public async Task<IHttpActionResult> AcceptSellTransaction([FromBody] TransactionDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest("Invalid data");
            }
            if (dto.BuyOffer.BuyerId != (Thread.CurrentPrincipal as UserPrincipal).Id)
            {
                return Unauthorized();
            }

            var sellOffer = _sellAssembler.DtoToEntity(dto.SellOffer);
            var buyOffer = _buyAssembler.DtoToEntity(dto.BuyOffer);
            var rating = dto.Rating;

            var result = await _transactionService.AcceptSellTransaction(sellOffer, buyOffer, rating);

            if (result == ErrorValue.ServerError)
            {
                return BadRequest("Transaction error");
            }
            else if(result == ErrorValue.TransactionAlreadyFinished)
            {
                return BadRequest("Transaction already finished");
            }
            return Ok();
        }
        [Authentication.Authentication, HttpsOnly]
        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] SellOfferFilterDto filter)
        {
            var id = (Thread.CurrentPrincipal as UserPrincipal).Id;
            var offers = await _service.GetData(filter.GetFilter(id));
            var dto = _sellAssembler.EntityToDto(offers);

            return Ok(dto);
        }
        [Authentication.Authentication, HttpsOnly]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] SellOfferDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest("Invalid data");
            }
            if (dto.SellerId != (Thread.CurrentPrincipal as UserPrincipal).Id)
            {
                return Unauthorized();
            }

            var offer = _sellAssembler.DtoToEntity(dto);

            var result = await _service.Add(offer);
            if (result == ErrorValue.ServerError)
            {
                return BadRequest("Transaction error");
            }
            else if(result == ErrorValue.AmountGreaterThanStock)
            {
                return BadRequest("Amount greater than stock");
            }
            dto = _sellAssembler.EntityToDto(offer);
            return Ok(dto);
        }
        [Authentication.Authentication, HttpsOnly]
        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody] SellOfferDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest("Invalid data");
            }
            if (dto.SellerId != (Thread.CurrentPrincipal as UserPrincipal).Id)
            {
                return Unauthorized();
            }

            var offer = _sellAssembler.DtoToEntity(dto);

            var result = await _service.Update(offer);

            if (result == ErrorValue.ServerError)
            {
                return BadRequest("Not found");
            }
            else if(result == ErrorValue.AmountGreaterThanStock)
            {
                return BadRequest("Amount greater than stock");
            }
            dto = _sellAssembler.EntityToDto(offer);
            return Ok(dto);
        }
        [Authentication.Authentication, HttpsOnly]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromBody] SellOfferDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest("Invalid data");
            }
            if (dto.SellerId != (Thread.CurrentPrincipal as UserPrincipal).Id)
            {
                return Unauthorized();
            }

            var offer = _sellAssembler.DtoToEntity(dto);

            var result = await _service.Delete(offer);

            if (result == ErrorValue.ServerError)
            {
                return BadRequest("Entity not found");
            }

            return Ok();
        }
    }
}