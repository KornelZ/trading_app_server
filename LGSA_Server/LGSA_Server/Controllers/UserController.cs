using LGSA.Model.Services;
using LGSA.Model.UnitOfWork;
using LGSA_Server.Authentication;
using LGSA_Server.Model;
using LGSA_Server.Model.Assemblers;
using LGSA_Server.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace LGSA_Server.Controllers
{
    public class UserController : ApiController
    {
        private ITwoWayAssembler<users_Authetication, AuthenticationDto> _assembler;
        private ITwoWayAssembler<users, UserDto> _userAssembler;
        private IDataService<users_Authetication> _service;

        public UserController(IUnitOfWorkFactory factory)
        {
            _service = new AuthenticationService(factory);
            _userAssembler = new UserAssembler(new AddressAssembler());
            _assembler = new AuthenticationAssembler(_userAssembler);
        }
        [Route("Login/"), HttpPost]
        public async Task<IHttpActionResult> Login([FromBody] AuthenticationDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var user = await _service.GetData(u => u.password == dto.Password && u.users1.UserName == dto.User.UserName);
            if(user.Count() == 1)
            {
                dto = _assembler.EntityToDto(user.First());
                return Ok(dto);
            }

            return BadRequest("Wrong username or password");
        }
        [Authentication.Authentication]
        [HttpGet]
        public async Task<IHttpActionResult> Get(int id)
        {
        
            var user = await _service.GetById(id);
            if(user == null)
            {
                return NotFound();
            }

            var info = _userAssembler.EntityToDto(user.users1);

            return Ok(info);
        }
        //Register
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] AuthenticationDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var user = _assembler.DtoToEntity(dto);

            var result = await _service.Add(user);
            if (result == false)
            {
                return BadRequest("Such user already exists");
            }

            dto = _assembler.EntityToDto(user);

            return Ok(dto);
        }
        [Authentication.Authentication]
        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody] AuthenticationDto dto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            if(dto.User.Id != (Thread.CurrentPrincipal as UserPrincipal).Id)
            {
                return BadRequest("Internal error");
            }

            var user = _assembler.DtoToEntity(dto);

            var result = await _service.Delete(user);
            if(result == false)
            {
                return BadRequest("Incorrect data");
            }
            result = await _service.Add(user);

            dto = _assembler.EntityToDto(user);
            return Ok(dto);
        }
    }
}