using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagementSystem.Models;

public class Match
{
    [Key]
    public int Id { get; set; }
    [Required]
    public Team HomeTeam { get; set; }
    [Required]
    public Team AwayTeam { get; set; }

    public Boolean IsPlayed { get; set; } = false;
    public int HomeTeamGoals { get; set; } = 0;
    public int AwayTeamGoals { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}