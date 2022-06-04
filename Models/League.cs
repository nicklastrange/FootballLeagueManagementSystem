using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagementSystem.Models;

public class League
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }

    public string OriginalName { get; set; } = "";
    public List<Team> TeamList { get; set; } = new List<Team>();
    public List<Match> Schedule { get; set; } = new List<Match>();
    public Boolean IsStarted { get; set; } = false;
    public Boolean IsFinished { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}