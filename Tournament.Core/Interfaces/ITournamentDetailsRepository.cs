using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;

namespace Tournament.Core.Interfaces;

public interface ITournamentDetailsRepository
{
    Task<IEnumerable<TournamentDetails>> GetAllAsync(bool includeGames);
    Task<TournamentDetails?> GetByIdAsync(int id, bool includeGames);
    Task<TournamentDetails> AddAsync(TournamentDetails tournament);
    Task<bool> UpdateAsync(TournamentDetails tournament);
    Task<bool> DeleteAsync(int id);
}
