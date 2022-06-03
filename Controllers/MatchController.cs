using FootballLeagueManagementSystem.Data;
using FootballLeagueManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagementSystem.Controllers;

public class MatchController : Controller
{
    private readonly ApplicationDbContext ctx;

    public MatchController(ApplicationDbContext ctx)
    {
        this.ctx = ctx;
    }
    // GET
    public IActionResult Edit(int? id)
    {
        var match = ctx.Matches.Include(x => x.HomeTeam)
            .Include(x => x.AwayTeam)
            .First(x => x.Id == id);
        return View(match);
    }
    
    [HttpPost]
    public IActionResult Edit(Match match)
    {
        
        var dbMatch = ctx.Matches.Include(x => x.AwayTeam)
            .Include(x => x.HomeTeam)
            .First(x => x.Id == match.Id);

        dbMatch.IsPlayed = true;
        dbMatch.HomeTeamGoals = match.HomeTeamGoals;
        dbMatch.AwayTeamGoals = match.AwayTeamGoals;
        ctx.Matches.Update(dbMatch);
        ctx.SaveChanges();
        TempData["success"] = "Match resolved successfully!";
        return RedirectToAction("Index", "League");
    }
}