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
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGames(int tournamentId)
    {
        var games = await _unitOfWork.GameRepository.GetAllAsync(tournamentId, trackChanges:false);
        var gamesDtos = _mapper.Map<IEnumerable<GameDto>>(games);
        return Ok(gamesDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameDto>> GetGame(int id, int tournamentId)
    {
        var game = await _unitOfWork.GameRepository.GetByIdAsync(id, trackChanges:false);
        if (game == null || game.TournamentId != tournamentId)
            return NotFound();
        var gameDto = _mapper.Map<GameDto>(game);
        return Ok(gameDto);
    }


    [HttpPost]
    public async Task<ActionResult<GameDto>> PostGame(int tournamentId, GameCreateDto gameCreateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var game = _mapper.Map<Game>(gameCreateDto);
        game.TournamentId = tournamentId;
        _unitOfWork.GameRepository.Create(game);
        await _unitOfWork.CompleteAsync();

        var gameDto = _mapper.Map<GameDto>(game);
        return CreatedAtAction(nameof(GetGame), new { tournamentId = tournamentId, id = game.Id }, gameDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutGame(int id, GameUpdateDto gameUpdateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (id != gameUpdateDto.Id)
        {
            return BadRequest();
        }

        var existingGame = await _unitOfWork.GameRepository.GetByIdAsync(id, trackChanges:true);
        if (existingGame == null)
        {
            return NotFound();
        }

        var game = _mapper.Map(gameUpdateDto, existingGame);

        await _unitOfWork.CompleteAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var existingGame = await _unitOfWork.GameRepository.GetByIdAsync(id, trackChanges:false);
        if (existingGame == null)
        {
            return NotFound();
        }

        _unitOfWork.GameRepository.Delete(existingGame);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchGame(int tournamentId, int id, JsonPatchDocument<GameUpdateDto> patchDoc)
    {
        if (patchDoc == null)
            return BadRequest();

        var tournament = await _unitOfWork.TournamentDetailsRepository.GetByIdAsync(tournamentId, includeGames: false, trackChanges:true);
        if (tournament == null)
            return NotFound();

        var gameToPatch = await _unitOfWork.GameRepository.GetByIdAsync(id, trackChanges:true);

        if (gameToPatch == null || gameToPatch.TournamentId != tournamentId) 
            return NotFound();

        var dto = _mapper.Map<GameUpdateDto>(gameToPatch);

        patchDoc.ApplyTo(dto, ModelState);

        if (!TryValidateModel(dto))
            return ValidationProblem(ModelState);

        _mapper.Map(dto, gameToPatch);


        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<GameDto>>> SearchTournamentsByTitle(int tournamentId, [FromQuery] string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Title parameter is required.");

        var games = await _unitOfWork.GameRepository.SearchByTitleAsync(tournamentId, title, trackChanges: false);

        if (games == null || !games.Any())
            return NotFound();

        var gameDtos = _mapper.Map<IEnumerable<GameDto>>(games);
        return Ok(gameDtos);
    }
}
