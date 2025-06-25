using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tournament.Core.Entities;

namespace Tournament.Data.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(TournamentApiContext context)
        {
            if (context.Tournaments.Any())
            {
                return;
            }

            var tournaments = new List<TournamentDetails>
            {
                new TournamentDetails
                {
                    Title = "Axelssons Cup 2025",
                    StartDate = DateTime.UtcNow.Date,
                    Games = new List<Game>
                    {
                        new Game { Title = "Quarterfinal 1", Time = DateTime.UtcNow.AddDays(1), TournamentId = 1 },
                        new Game { Title = "Quarterfinal 2", Time = DateTime.UtcNow.AddDays(2), TournamentId = 1 }
                    }
                },
                new TournamentDetails
                {
                    Title = "Aros Cup 2025",
                    StartDate = DateTime.UtcNow.Date.AddMonths(1),
                    Games = new List<Game>
                    {
                        new Game { Title = "Semifinal 1", Time = DateTime.UtcNow.AddMonths(1).AddDays(1), TournamentId = 2 },
                        new Game { Title = "Semifinal 2", Time = DateTime.UtcNow.AddMonths(1).AddDays(2), TournamentId = 2 }
                    }
                }
            };

            context.Tournaments.AddRange(tournaments);
            await context.SaveChangesAsync();
        }
    }
}

