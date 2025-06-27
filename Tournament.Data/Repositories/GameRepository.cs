using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tournament.Core.Entities;
using Tournament.Core.Interfaces;
using Tournament.Data.Data;
using Tournament.Data.Repositories;

public class GameRepository : RepositoryBase<Game>, IGameRepository
{
    //private readonly TournamentApiContext _context;

    public GameRepository(TournamentApiContext context) : base(context)
    {
        //_context = context;
    }

    public async Task<IEnumerable<Game>> GetAllAsync(int tournamentId, bool trackChanges = false)
    {
        return await FindAll(trackChanges).Where(g => g.TournamentId == tournamentId).ToListAsync();
    }

    public async Task<Game?> GetByIdAsync(int id, bool trackChanges)
    {
        return await FindByCondition(g => g.Id.Equals(id), trackChanges).FirstOrDefaultAsync(); ;
    }

    public async Task<IEnumerable<Game>> SearchByTitleAsync(string title, bool trackChanges)
    {
        return await FindByCondition(g => g.Title.ToLower().Contains(title.ToLower()), trackChanges).ToListAsync(); 
    }

}
