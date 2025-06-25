using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tournament.Core.Dto;
using Tournament.Core.Entities;
using Tournament.Core.Interfaces;

[Route("api/tournaments/{tournamentId}/games")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GameController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGames()
    {
        var games = await _unitOfWork.GameRepository.GetAllAsync();
        var gamesDtos = _mapper.Map<IEnumerable<GameDto>>(games);
        return Ok(gamesDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Game>> GetGame(int id)
    {
        var game = await _unitOfWork.GameRepository.GetByIdAsync(id);
        if (game == null)
            return NotFound();
        var gameDto = _mapper.Map<GameDto>(game);
        return Ok(game);
    }

    [HttpPost]
    public async Task<ActionResult<GameDto>> PostGame(GameCreateDto gameCreateDto)
    {
        var game = _mapper.Map<Game>(gameCreateDto);
        await _unitOfWork.GameRepository.AddAsync(game);
        await _unitOfWork.CompleteAsync();

        var gameDto = _mapper.Map<GameDto>(game);
        return CreatedAtAction(nameof(GetGame), new { id = game.Id }, gameDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutGame(int id, GameUpdateDto gameUpdateDto)
    {
        if (!await _unitOfWork.GameRepository.ExistsAsync(id))
            return NotFound();

        var game = _mapper.Map<Game>(gameUpdateDto);
        game.Id = id;

        var success = await _unitOfWork.GameRepository.UpdateAsync(game);
        if (!success) return StatusCode(500, "Could not update the game.");

        await _unitOfWork.CompleteAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var success = await _unitOfWork.GameRepository.DeleteAsync(id);
        if (!success) return NotFound();

        await _unitOfWork.CompleteAsync();
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchGame(int tournamentId, int id, JsonPatchDocument<GameUpdateDto> patchDoc)
    {
        if (patchDoc == null)
            return BadRequest();

        var tournament = await _unitOfWork.TournamentDetailsRepository.GetByIdAsync(tournamentId, includeGames: false);
        if (tournament == null)
            return NotFound();

        var gameToPatch = await _unitOfWork.GameRepository.GetByIdAsync(id);

        if (gameToPatch == null || gameToPatch.TournamentId != tournamentId) 
            return NotFound();

        var dto = _mapper.Map<GameUpdateDto>(gameToPatch);

        patchDoc.ApplyTo(dto, ModelState);

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        _mapper.Map(dto, gameToPatch);


        await _unitOfWork.CompleteAsync();

        return NoContent();
    }
}
