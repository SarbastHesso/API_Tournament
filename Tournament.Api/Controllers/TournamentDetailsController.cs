using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
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
        private readonly IServiceManager _serviceManager;

        public TournamentDetailsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // GET: api/TournamentDetails
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TournamentDetailsDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TournamentDetailsDto>>> GetAllTournamentDetails([FromQuery] bool includeGames)
        {
            var tournamentsDtos = await _serviceManager.TournamentDetailsService.GetAllAsync(includeGames, trackChanges: false);
            if (tournamentsDtos == null)
            {
                return NotFound();
            }

            return Ok(tournamentsDtos);
        }

        // GET: api/TournamentDetails/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TournamentDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TournamentDetailsDto>> GetTournamentDetails(int id, [FromQuery] bool includeGames)
        {

            var tournamentDto = await _serviceManager.TournamentDetailsService.GetByIdAsync(id, includeGames, trackChanges:false);

            if (tournamentDto == null)
            {
                return NotFound();
            }

            return Ok(tournamentDto);
        }


        // PUT: api/TournamentDetails/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]  
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        [ProducesResponseType(StatusCodes.Status404NotFound)]   
        public async Task<IActionResult> PutTournamentDetails(int id, [FromBody] TournamentDetailsUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _serviceManager.TournamentDetailsService.UpdateAsync(id, updateDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            
        }

        // POST: api/TournamentDetails
        [HttpPost]
        [ProducesResponseType(typeof(TournamentDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PostTournamentDetails([FromBody] TournamentDetailsCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tournamentDto = await _serviceManager.TournamentDetailsService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetTournamentDetails), new { id = tournamentDto.Id }, tournamentDto);
        }

        // DELETE: api/TournamentDetails/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTournamentDetails(int id)
        {
            try
            {
                await _serviceManager.TournamentDetailsService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTournamentDetails(int id, [FromBody] JsonPatchDocument<TournamentDetailsUpdateDto> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var patchedDto = await _serviceManager.TournamentDetailsService.GetPatchedDtoAsync(id, patchDoc);

            if (patchedDto == null)
                return NotFound();

            TryValidateModel(patchedDto);

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            await _serviceManager.TournamentDetailsService.ApplyPatchedDtoAsync(id, patchedDto);

            return NoContent();
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<TournamentDetailsDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TournamentDetailsDto>>> SearchTournamentsByTitle([FromQuery] string title, [FromQuery] bool includeGames)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("Title is required");

            var tournaments = await _serviceManager.TournamentDetailsService.SearchByTitle(title, includeGames, trackChanges: false);

            if (tournaments == null)
            {
                return NotFound();
            }

            return Ok(tournaments);
        }
    }
}

