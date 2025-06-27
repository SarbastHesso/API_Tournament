using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;

namespace Tournament.Core.Interfaces;

public interface IGameRepository
{
    //Task<IEnumerable<Game>> GetAllAsync(int tournamentId);
    //Task<Game?> GetByIdAsync(int id);
    //Task<IEnumerable<Game?>> GetByTitleAsync(string title);
    //Task<Game> AddAsync(Game game);
    //Task<bool> UpdateAsync(Game game);
    //Task<bool> DeleteAsync(int id);
    //Task<bool> ExistsAsync(int id);
    Task<IEnumerable<Game>> GetAllAsync(int tournamentId, bool trackChanges);
    Task<Game?> GetByIdAsync(int id, bool trackChanges);
    Task<IEnumerable<Game>> SearchByTitleAsync(string title, bool trackChanges);
    void Create(Game game);
    void Delete(Game game);
}
