using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using LGSA_Server.Model;
using LGSA.Model.Services;
using LGSA.Model.UnitOfWork;
using System.Threading.Tasks;
using LGSA_Server.Model.DTO;
using LGSA_Server.Model.Assemblers;
using System.Threading;
using LGSA_Server.Authentication;
using LGSA_Server.Model.DTO.Filters;
using LGSA_Server.Model.Enums;

namespace LGSA_Server.Controllers
{
    public class ProductController : ApiController
    {
        private IDataService<product> _service;
        private ITwoWayAssembler<product, ProductDto> _assembler;


        public ProductController(IUnitOfWorkFactory factory)
        {
            _service = new ProductService(factory);
            _assembler = new ProductAssembler(new ConditionAssembler(),
                                                     new GenreAssembler(),
                                                     new ProductTypeAssembler());

        }
        // GET: api/Product
        [Authentication.Authentication]
        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] ProductFilterDto filter)
        {
            var id = (Thread.CurrentPrincipal as UserPrincipal).Id;
            var products = await _service.GetData(filter.GetFilter(id));
            var dto = _assembler.EntityToDto(products);

            return Ok(dto);
        }
        [Authentication.Authentication]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] ProductDto dto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest("Invalid data");
            }
            if (dto.ProductOwner != (Thread.CurrentPrincipal as UserPrincipal).Id)
            {
                return Unauthorized();
            }

            var product = _assembler.DtoToEntity(dto);

            var result = await _service.Add(product);

            if(result == ErrorValue.EntityExists)
            {
                return BadRequest("Such product already exists.");
            }
            else if(result == ErrorValue.ServerError)
            {
                return BadRequest("Transaction error");
            }

            dto = _assembler.EntityToDto(product);
            return Ok(dto);
        }
        [Authentication.Authentication]
        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody] ProductDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest("Invalid data");
            }
            if (dto.ProductOwner != (Thread.CurrentPrincipal as UserPrincipal).Id)
            {
                return Unauthorized();
            }

            var product = _assembler.DtoToEntity(dto);

            var result = await _service.Update(product);

            if(result == ErrorValue.ServerError)
            {
                return BadRequest("Entity not found");
            }
            else if(result == ErrorValue.AmountGreaterThanStock)
            {
                return BadRequest("Amount greater than stock");
            }

            dto = _assembler.EntityToDto(product);
            return Ok(dto);
        }

    }
}