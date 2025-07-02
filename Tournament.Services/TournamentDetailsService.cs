using AutoMapper;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Dto;
using Tournament.Core.Entities;
using Tournament.Core.Interfaces;
using Microsoft.AspNetCore.JsonPatch;

namespace Tournament.Services;

public class TournamentDetailsService: ITournamentDetailsService
{
    private IUnitOfWork _unitOfWork;
    private readonly IMapper  _mapper;
    public TournamentDetailsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;   
    }

    public async Task<IEnumerable<TournamentDetailsDto>> GetAllAsync(bool includeGames = false, bool trackChanges = false)
    {
        var tournaments = await _unitOfWork.TournamentDetailsRepository.GetAllAsync(includeGames, trackChanges);
        var dto = _mapper.Map<IEnumerable<TournamentDetailsDto>>(tournaments);
        return dto;
    }

    public async Task<TournamentDetailsDto> GetByIdAsync(int id, bool includeGames = false, bool trackChanges = false)
    {
        var tournament = await _unitOfWork.TournamentDetailsRepository.GetByIdAsync(id, includeGames, trackChanges);
        if (tournament == null)
            throw new KeyNotFoundException($"Tournament with id {id} not found");

        var dto = _mapper.Map<TournamentDetailsDto>(tournament);
        return dto;
    }

    public async Task<TournamentDetailsDto> CreateAsync(TournamentDetailsCreateDto createDto)
    {
        var tournament = _mapper.Map<TournamentDetails>(createDto);
        _unitOfWork.TournamentDetailsRepository.Create(tournament);
        await _unitOfWork.CompleteAsync();
        var tournamentDto = _mapper.Map<TournamentDetailsDto>(tournament);
        return tournamentDto;
    }

    public async Task DeleteAsync(int id)
    {
        var existingTournament = await _unitOfWork.TournamentDetailsRepository.GetByIdAsync(id, includeGames: false, trackChanges: false);
        if (existingTournament == null)
            throw new KeyNotFoundException($"Tournament with id {id} not found");

        _unitOfWork.TournamentDetailsRepository.Delete(existingTournament);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateAsync(int id, TournamentDetailsUpdateDto updateDto)
    {
        var existingTournament = await _unitOfWork.TournamentDetailsRepository.GetByIdAsync(id, includeGames: false, trackChanges: true);
        if (existingTournament == null)
            throw new KeyNotFoundException($"Tournament with id {id} not found");

        _mapper.Map(updateDto, existingTournament);
        await _unitOfWork.CompleteAsync();
    }
    public async Task<TournamentDetailsUpdateDto> GetPatchedDtoAsync(int id, JsonPatchDocument<TournamentDetailsUpdateDto> patchDoc)
    {
        var entity = await _unitOfWork.TournamentDetailsRepository
            .GetByIdAsync(id, includeGames: false, trackChanges: true);

        if (entity == null)
            return null;

        var dto = _mapper.Map<TournamentDetailsUpdateDto>(entity);
        patchDoc.ApplyTo(dto);

        return dto;
    }

    public async Task ApplyPatchedDtoAsync(int id, TournamentDetailsUpdateDto patchedDto)
    {
        var entity = await _unitOfWork.TournamentDetailsRepository
            .GetByIdAsync(id, includeGames: false, trackChanges: true);

        if (entity == null)
            throw new KeyNotFoundException($"Tournament with id {id} not found");

        _mapper.Map(patchedDto, entity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<IEnumerable<TournamentDetailsDto>> SearchByTitle(string title, bool includeGames = false, bool trackChanges = false)
    {
        if (title == null) throw new ArgumentNullException("title");

        var tournaments = await _unitOfWork.TournamentDetailsRepository.SearchByTitleAsync(title, includeGames, trackChanges);
        var dto = _mapper.Map<IEnumerable<TournamentDetailsDto>>(tournaments);
        return dto;
    }

}
