using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;

namespace Tournament.Core.Interfaces;

public interface IGameRepository
{
    Task<IEnumerable<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(int id);
    Task<Game> AddAsync(Game game);
    Task<bool> UpdateAsync(Game game);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
