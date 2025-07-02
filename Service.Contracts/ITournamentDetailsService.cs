using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Dto;
using Tournament.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace Service.Contracts;

public interface ITournamentDetailsService
{
    Task<IEnumerable<TournamentDetailsDto>> GetAllAsync(bool includeGames, bool trackChanges);
    Task<TournamentDetailsDto> GetByIdAsync(int id , bool includeGames, bool trackChanges);
    Task<TournamentDetailsDto> CreateAsync(TournamentDetailsCreateDto createDto);
    Task DeleteAsync(int id);
    Task UpdateAsync(int id, TournamentDetailsUpdateDto updatedDto);
    Task<TournamentDetailsUpdateDto> GetPatchedDtoAsync(int id, JsonPatchDocument<TournamentDetailsUpdateDto> patchDoc);
    Task ApplyPatchedDtoAsync(int id, TournamentDetailsUpdateDto patchedDto);
    Task<IEnumerable<TournamentDetailsDto>> SearchByTitle(string title, bool includeGames, bool trackChanges);


}
