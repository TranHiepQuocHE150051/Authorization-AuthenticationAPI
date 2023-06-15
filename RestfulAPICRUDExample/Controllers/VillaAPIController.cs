using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestfulAPICRUDExample.Data;
using RestfulAPICRUDExample.Logging;
using RestfulAPICRUDExample.Models;
using RestfulAPICRUDExample.Models.DTO;
using RestfulAPICRUDExample.Repository.IRepository;
using System.Net;

namespace RestfulAPICRUDExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {

        private readonly ILogging _logger;
        private readonly IMapper _mapper;
        private readonly IVillaRepository _dbVilla;
        protected APIResponse _response;
        public VillaAPIController(ILogging logger, IVillaRepository villaRepository,IMapper mapper)
        {
            _logger = logger;
            _dbVilla= villaRepository;
            _mapper = mapper;
            this._response = new();
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>>   GetVillas()
        {

            try
            {
                _logger.Log("Getting all villas", "");
                IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }
            return _response;
            
        }
        [HttpGet("{id}",Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>>  GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.Log("Get villa error with id " + id,"error");
                return BadRequest("Id not valid");
            }
            var villa = await _dbVilla.GetAsync(u=>u.Id==id);
            if (villa == null)
            {
                return NotFound("Villa not found");
            }
            else
            {
                return Ok(_mapper.Map<VillaDTO>(villa));
            }
        }
        [HttpPost]
        [Authorize(Roles ="admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>>  CreateVilla([FromBody]CreateVillaDTO villaDTO)
        {
            //Explicit modelstate validation

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Custom ModelState Validations
            foreach (var item in VillaStore.villaList)
            {
                if (villaDTO.Name.ToLower().Equals(item.Name.ToLower()))
                {
                    ModelState.AddModelError("Duplicate Name Error", "Villa already exist");
                    return BadRequest(ModelState);
                }
            }

            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }
            //Villa model = new Villa()
            //{
            //    Name = villaDTO.Name,
            //    Amenity = villaDTO.Amenity,
            //    Details = villaDTO.Details,
            //    ImageUrl = villaDTO.ImageUrl,
            //    Occupancy = villaDTO.Occupancy,
            //    Rate = villaDTO.Rate,
            //    Sqft = villaDTO.Sqft
            //};

            Villa model =_mapper.Map<Villa>(villaDTO);   
            await _dbVilla.CreateAsync(model);

            return Ok(model);
        }
        [HttpDelete("{id}",Name ="DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteVillaAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest("Id not valid");
            }
            var villa = await _dbVilla.GetAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound("Villa not found");
            }
            await _dbVilla.RemoveAsync(villa);
            
            return NoContent();
        }
        [HttpPut("{id}",Name ="UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult>  UpdateVilla(int id, [FromBody]UpdateVillaDTO villaDTO)
        {
            if (villaDTO == null || id!=villaDTO.Id)
            {
                return BadRequest();
            }
            Villa model = _mapper.Map<Villa>(villaDTO);
            await _dbVilla.UpdateAsync(model);
            
            return NoContent();
        }
        [HttpPatch("{id}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult>  UpdatePartialVilla(int id, JsonPatchDocument<UpdateVillaDTO> patchDTO)
        {
            if (patchDTO == null || id ==0)
            {
                return BadRequest();
            }
            var villa = await _dbVilla.GetAsync(x => x.Id == id,tracked:false);
            if (villa == null)
            {
                return BadRequest();

            }
            UpdateVillaDTO villaDTO = _mapper.Map<UpdateVillaDTO>(villa);
            patchDTO.ApplyTo(villaDTO, ModelState);
            Villa model = _mapper.Map<Villa>(villaDTO);
            await _dbVilla.UpdateAsync(model);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
