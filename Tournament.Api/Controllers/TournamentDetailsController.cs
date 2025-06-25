using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tournament.Core.Dto;
using Tournament.Core.Entities;
using Tournament.Core.Interfaces;
using Tournament.Data.Repositories;

namespace Tournament.Api.Controllers
{
    [Route("api/tournaments")]
    [ApiController]
    public class TournamentDetailsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TournamentDetailsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/TournamentDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentDetailsDto>>> GetAllTournamentDetails([FromQuery] bool includeGames)
        {
            var tournaments = await _unitOfWork.TournamentDetailsRepository.GetAllAsync(includeGames);
            var dto = _mapper.Map<IEnumerable<TournamentDetailsDto>>(tournaments);
            return Ok(dto);
        }

        // GET: api/TournamentDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentDetailsDto>> GetTournamentDetails(int id, [FromQuery] bool includeGames)
        {
            var tournament = await _unitOfWork.TournamentDetailsRepository.GetByIdAsync(id, includeGames);

            if (tournament == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<TournamentDetailsDto>(tournament);

            return Ok(dto);
        }

        // PUT: api/TournamentDetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournamentDetails(int id, [FromBody] TournamentDetailsUpdateDto updateDto)
        {
            var entity = _mapper.Map<TournamentDetails>(updateDto);
            entity.Id = id;

            var updated = await _unitOfWork.TournamentDetailsRepository.UpdateAsync(entity);
            if (!updated)
            {
                return NotFound();
            }

            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        // POST: api/TournamentDetails
        [HttpPost]
        public async Task<ActionResult> PostTournamentDetails([FromBody] TournamentDetailsCreateDto createDto)
        {
            var tournament = _mapper.Map<TournamentDetails>(createDto);
            await _unitOfWork.TournamentDetailsRepository.AddAsync(tournament);
            await _unitOfWork.CompleteAsync();
            var tournamentDto = _mapper.Map<TournamentDetailsDto>(tournament);
            return CreatedAtAction(nameof(GetTournamentDetails), new { id = tournament.Id }, tournamentDto);
        }

        // DELETE: api/TournamentDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournamentDetails(int id)
        {
            var deleted = await _unitOfWork.TournamentDetailsRepository.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTournamentDetails(int id, [FromBody] JsonPatchDocument<TournamentDetailsUpdateDto> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var tournamentToPatch = await _unitOfWork.TournamentDetailsRepository.GetByIdAsync(id, includeGames:false);
            if (tournamentToPatch == null) 
                return NotFound();

            var dto = _mapper.Map<TournamentDetailsUpdateDto>(tournamentToPatch);

            patchDoc.ApplyTo(dto, ModelState);

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(dto, tournamentToPatch);


            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}

