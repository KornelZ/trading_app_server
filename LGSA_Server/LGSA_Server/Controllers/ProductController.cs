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
        private IDataService<product> service;
        private IAssembler<product, ProductDto> assembler;

        public ProductController(IUnitOfWorkFactory factory)
        {
            service = new ProductService(factory);
            assembler = new ProductAssembler();
        }
        // GET: api/Product
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var product = await service.GetData(null);
            return Ok(assembler.EntityToDto(product));
        }
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] ProductDto dto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var product = assembler.DtoToEntity(dto);

            var result = await service.Add(product);

            if(result == false)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }
        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromBody] ProductDto dto)
        {
            var product = assembler.DtoToEntity(dto);

            var result = await service.Delete(product);

            if(result == false)
            {
                return NotFound();
            }
            return Ok();
        }

    }
}