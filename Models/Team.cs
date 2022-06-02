using System.ComponentModel.DataAnnotations;

namespace FootballLeagueManagementSystem.Models;

public class Team
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}