using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tournament.Core.Entities;
using Tournament.Core.Interfaces;
using Tournament.Data.Data;

public class GameRepository : IGameRepository
{
    private readonly TournamentApiContext _context;

    public GameRepository(TournamentApiContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await _context.Games.ToListAsync();
    }

    public async Task<Game?> GetByIdAsync(int id)
    {
        return await _context.Games.FindAsync(id);
    }

    public async Task<Game> AddAsync(Game game)
    {
        await _context.Games.AddAsync(game);
        return game;
    }

    public async Task<bool> UpdateAsync(Game game)
    {
        var existingGame = await _context.Games.FindAsync(game.Id);
        if (existingGame == null)
            return false;

        // Update properties
        existingGame.Title = game.Title;
        existingGame.Time = game.Time;
        existingGame.TournamentId = game.TournamentId;

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var game = await _context.Games.FindAsync(id);
        if (game != null)
        {
            _context.Games.Remove(game);
            return true;
        }
        return false;

    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Games.AnyAsync(g => g.Id == id);
    }
}
