using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/trails")]

    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecTrails")]

    public class TrailController : ControllerBase
    {
        private readonly  ITrailRepository _trailRepo;
        private readonly IMapper _mapper;

        public TrailController(ITrailRepository trailRepo , IMapper mapper)
        {
            _trailRepo = trailRepo;
            _mapper = mapper;
        }


        /// <summary>
        /// Get List of All Trails
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TrailDto>))]
        public IActionResult GetTrails()
        {
            var opjList = _trailRepo.GetTrails();

            var opjDto = new List<TrailDto>();

            foreach(var opj in opjList)
            {
                opjDto.Add(_mapper.Map<TrailDto>(opj));
            }

            return Ok(opjDto);
        }


        /// <summary>
        /// Get Individual Trail
        /// </summary>
        /// <param name="trailId">The Id of Trail</param>
        /// <returns></returns>
        [HttpGet("{trailId:int}",Name = "GetTrail")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = "Admin")]
        public IActionResult GetTrail(int trailId)
        {
            var opj = _trailRepo.GetTrail(trailId);

            if(opj == null)
            {
                return NotFound();
            }

            var opjDto = _mapper.Map<TrailDto>(opj);

            return Ok(opjDto);
        }




        /// <summary>
        /// Get Trails in Individual National Park
        /// </summary>
        /// <param name="nationalParkId">The Id of National Park</param>
        /// <returns></returns>
        [HttpGet("[action]/{nationalParkId:int}")]
        [ProducesResponseType(200, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrailsNationalPark(int nationalParkId)
        {
            var opjList = _trailRepo.GetTrailsInNationalPark(nationalParkId);

            if (opjList == null)
            {
                return NotFound();
            }

            var opjDto = new List<TrailDto>();

            foreach(var opj in opjList)
            {
                opjDto.Add(_mapper.Map<TrailDto>(opj));
            }

      

            return Ok(opjDto);
        }



        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateTrail([FormBody] TrailCreateDto trailDto)
        {
            if(trailDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_trailRepo.TrailExists(trailDto.Name))
            {
                ModelState.AddModelError("", "Trail Exists");
                return StatusCode(404, ModelState);
            }

            var trailOpj = _mapper.Map<Trail>(trailDto);

            if(!_trailRepo.CraeteTrail(trailOpj))
            {
                ModelState.AddModelError("", $"Somthing wrong when saving the {trailOpj.Name}");
                return StatusCode(404, ModelState);
            }

            return CreatedAtRoute("GetTrail", new { trailId = trailOpj.Id }, trailOpj);

        }

        [HttpPatch("{trailId:int}", Name = "UpdateTrail")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTrail(int trailId , [FormBody] TrailUpdateDto trailDto)
        {
            if (trailDto == null || trailId != trailDto.Id)
            {
                return BadRequest(ModelState);
            }

            var trailOpj = _mapper.Map<Trail>(trailDto);

            if (!_trailRepo.UpdateTrail(trailOpj))
            {
                ModelState.AddModelError("", $"Somthing wrong when Update the {trailOpj.Name}");
                return StatusCode(404, ModelState);
            }

            return NoContent();
        }




        [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail(int trailId)
        {
            if (!_trailRepo.TrailExists(trailId))
            {
                return NotFound();
            }

            var trailOpj = _trailRepo.GetTrail(trailId);

            if (!_trailRepo.DeleteTrail(trailOpj))
            {
                ModelState.AddModelError("", $"Somthing wrong when delete the {trailOpj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
