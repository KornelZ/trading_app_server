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
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var products = await _service.GetData(null);
            var dto = _assembler.EntityToDto(products);

            return Ok(_assembler.EntityToDto(products));
        }
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] ProductDto dto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var product = _assembler.DtoToEntity(dto);

            var result = await _service.Add(product);

            if(result == false)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }
        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromBody] ProductDto dto)
        {
            var product = _assembler.DtoToEntity(dto);

            var result = await _service.Delete(product);

            if(result == false)
            {
                return NotFound();
            }
            return Ok();
        }

    }
}