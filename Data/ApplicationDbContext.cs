using FootballLeagueManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagementSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<Team> Teams { get; set; }
    public DbSet<League> Leagues { get; set; }
    public DbSet<Match> Matches { get; set; }
}