using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagementSystem.Models;

public class League
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }

    public HashSet<Team> TeamSet { get; set; } = new HashSet<Team>();
    public HashSet<Match> Schedule { get; set; } = new HashSet<Match>();
    public Boolean IsStarted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}