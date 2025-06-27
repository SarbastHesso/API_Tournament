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
            var tournaments = await _unitOfWork.TournamentDetailsRepository.GetAllAsync(includeGames, trackChanges:false);
            var dto = _mapper.Map<IEnumerable<TournamentDetailsDto>>(tournaments);
            return Ok(dto);
        }

        // GET: api/TournamentDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentDetailsDto>> GetTournamentDetails(int id, [FromQuery] bool includeGames)
        {
            var tournament = await _unitOfWork.TournamentDetailsRepository.GetByIdAsync(id, includeGames, trackChanges: false);

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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (id != updateDto.Id)
            {
                return BadRequest();
            }

            var existingTournament = await _unitOfWork.TournamentDetailsRepository.GetByIdAsync(id, includeGames: false, trackChanges:true);
            if (existingTournament == null)
            {
                return NotFound();
            }

            _mapper.Map(updateDto, existingTournament);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        // POST: api/TournamentDetails
        [HttpPost]
        public async Task<ActionResult> PostTournamentDetails([FromBody] TournamentDetailsCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var tournament = _mapper.Map<TournamentDetails>(createDto);
            _unitOfWork.TournamentDetailsRepository.Create(tournament);
            await _unitOfWork.CompleteAsync();
            var tournamentDto = _mapper.Map<TournamentDetailsDto>(tournament);
            return CreatedAtAction(nameof(GetTournamentDetails), new { id = tournament.Id }, tournamentDto);
        }

        // DELETE: api/TournamentDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournamentDetails(int id)
        {
            var existingTournament = await _unitOfWork.TournamentDetailsRepository.GetByIdAsync(id, includeGames: false, trackChanges: false);

            if (existingTournament == null)
            {
                return NotFound();
            }
            _unitOfWork.TournamentDetailsRepository.Delete(existingTournament);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTournamentDetails(int id, [FromBody] JsonPatchDocument<TournamentDetailsUpdateDto> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var tournamentToPatch = await _unitOfWork.TournamentDetailsRepository.GetByIdAsync(id, includeGames:false, trackChanges:true);
            if (tournamentToPatch == null) 
                return NotFound();

            var dto = _mapper.Map<TournamentDetailsUpdateDto>(tournamentToPatch);

            patchDoc.ApplyTo(dto, ModelState);

            if (!TryValidateModel(dto))
                return ValidationProblem(ModelState);


            _mapper.Map(dto, tournamentToPatch);


            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TournamentDetailsDto>>> SearchTournamentsByTitle([FromQuery] string title, [FromQuery] bool includeGames)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("Title is required");

            var tournament = await _unitOfWork.TournamentDetailsRepository.SearchByTitleAsync(title, includeGames, trackChanges: false);

            if (tournament == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<IEnumerable<TournamentDetailsDto>>(tournament);

            return Ok(dto);
        }
    }
}

