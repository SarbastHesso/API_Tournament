using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;
using Tournament.Core.Interfaces;
using Tournament.Data.Data;

namespace Tournament.Data.Repositories;

public class TournamentDetailsRepository : ITournamentDetailsRepository
{
    private readonly TournamentApiContext _context;

    public TournamentDetailsRepository(TournamentApiContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TournamentDetails>> GetAllAsync(bool includeGames=false)
    {
        return includeGames 
            ? await _context.Tournaments.Include(t => t.Games).ToListAsync() 
            : await _context.Tournaments.ToListAsync();
    }

    public async Task<TournamentDetails?> GetByIdAsync(int id, bool includeGames=false)
    {
        return includeGames
            ? await _context.Tournaments.Include(t => t.Games).FirstOrDefaultAsync(t => t.Id.Equals(id))
            : await _context.Tournaments.FirstOrDefaultAsync(t => t.Id.Equals(id));
    }

    public async Task<TournamentDetails> AddAsync(TournamentDetails tournament)
    {
        await _context.Tournaments.AddAsync(tournament);
        return tournament;
    }

    public async Task<bool> UpdateAsync(TournamentDetails tournament)
    {
        var existingTournament = await _context.Tournaments
            .FirstOrDefaultAsync(t => t.Id.Equals(tournament.Id));

        if (existingTournament == null)
        {
            return false; // not found
        }

        // Update fields manually (safer, prevents overwriting everything)
        existingTournament.Title = tournament.Title;
        existingTournament.StartDate = tournament.StartDate;
        // add any other fields here

        return true; // success
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tournament = await _context.Tournaments.FindAsync(id);
        if (tournament != null)
        {
            _context.Tournaments.Remove(tournament);
            return true;
        }
        return false;
    }
}

