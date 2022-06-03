using FootballLeagueManagementSystem.Data;
using FootballLeagueManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueManagementSystem.Controllers;

public class LeagueController : Controller
{
    private readonly ApplicationDbContext ctx;

    public LeagueController(ApplicationDbContext ctx)
    {
        this.ctx = ctx;
    }
    // GET
    public IActionResult Index()
    {
        List<League> leaguesList = ctx.Leagues.Include(x => x.TeamSet).ToList();
        return View(leaguesList);
    }
    
    public IActionResult Table(int? id)
    {
        League league = ctx.Leagues.Include(x => x.Schedule).Include(x => x.TeamSet).First(league => league.Id == id);

        List<TeamWithMatches> teamWithMatchesList = new List<TeamWithMatches>();
        foreach (var t in league.TeamSet)
        {
            TeamWithMatches teamWithMatches = new TeamWithMatches();
            List<Match> matches = league.Schedule
                .Where(match => match.IsPlayed && (match.AwayTeam == t || match.HomeTeam == t))
                .ToList();
            teamWithMatches.Team = t;
            teamWithMatches.Matches = matches;
            teamWithMatchesList.Add(teamWithMatches);
        }
        return View(teamWithMatchesList);
    }
    
    public IActionResult Start(int? id)
    {
        League league = ctx.Leagues.Include(x => x.Schedule).Include(x => x.TeamSet).First(league => league.Id == id);
        if (league == null)
        {
            return NotFound();
        }

        HashSet<Match> schedule = new HashSet<Match>();
        foreach (var team in league.TeamSet)
        {
            HashSet<Team> teamsWithoutCurrentTeam = new HashSet<Team>(league.TeamSet);
            teamsWithoutCurrentTeam.Remove(team);
            foreach (var team1 in teamsWithoutCurrentTeam)
            {
                var homeMatch = new Match();
                var awayMatch = new Match();
                homeMatch.HomeTeam = team;
                homeMatch.AwayTeam = team1;
                awayMatch.AwayTeam = team;
                awayMatch.HomeTeam = team1;
                if (schedule
                    .Any(x => (x.HomeTeam == homeMatch.HomeTeam && x.AwayTeam == homeMatch.AwayTeam) 
                              || (x.HomeTeam == awayMatch.HomeTeam && x.AwayTeam == awayMatch.AwayTeam)))
                {
                    continue;
                }
                schedule.Add(homeMatch);
                schedule.Add(awayMatch);
            }
        }
        
        league.Schedule = schedule;
        league.IsStarted = true;
        ctx.Update(league);
        ctx.SaveChanges();
        TempData["success"] = "Season started successfully!";
        return RedirectToAction("Index");
    }

    public IActionResult Schedule(int? id)
    {
        var matches = ctx.Matches.Include(x => x.AwayTeam)
            .Include(x => x.HomeTeam)
            .Where(x => x.HomeTeam.Id == id || x.AwayTeam.Id == id).ToHashSet();
        if (matches == null)
        {
            return NotFound();
        }

        return View(matches);
    }

    // GET
    public IActionResult Create()
    {
        return View();
    }
    
    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(League league)
    {
        if (!ModelState.IsValid)
        {
            return View(league);
        }
        ctx.Leagues.Add(league);
        ctx.SaveChanges();
        TempData["success"] = "League created successfully!";
        return RedirectToAction("Index"); //we can add second parameter which defines controller of the action
    }
    
    // GET
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var leagueFromDb = ctx.Leagues.Find(id);

        if (leagueFromDb == null)
        {
            return NotFound();
        }
        
        return View(leagueFromDb);
    }
    
    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(League league)
    {
        if (!ModelState.IsValid)
        {
            return View(league);
        }
        ctx.Leagues.Update(league);
        ctx.SaveChanges();
        TempData["success"] = "League updated successfully!";
        return RedirectToAction("Index"); //we can add second parameter which defines controller of the action
    }
    
    // GET
    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var leagueFromDb = ctx.Leagues.Find(id);

        if (leagueFromDb == null)
        {
            return NotFound();
        }
        
        return View(leagueFromDb);
    }
    
    // POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int? id)
    {
        var league = ctx.Leagues.Find(id);

        if (league == null)
        {
            return NotFound();
        }

        ctx.Leagues.Remove(league);
        ctx.SaveChanges();
        TempData["success"] = "League removed successfully!";
        return RedirectToAction("Index"); //we can add second parameter which defines controller of the action
    }
    
    // GET
    public IActionResult AddTeam(int id)
    {
        IdWithTeam idWithTeam = new IdWithTeam(id);
        return View(idWithTeam);
    }
    
    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddTeam(IdWithTeam idWithTeam)
    {
        if (!ModelState.IsValid)
        {
            return View(idWithTeam);
        }

        League league = ctx.Leagues.Find(idWithTeam.id);
        
        if (league == null)
        {
            return NotFound();
        }

        league.TeamSet.Add(idWithTeam.team);
        ctx.Leagues.Update(league);
        ctx.SaveChanges();
        TempData["success"] = "Team added to a league successfully!";
        return RedirectToAction("Index"); //we can add second parameter which defines controller of the action
    }
}

public class IdWithTeam
{
    public int id { get; set; }
    public Team team { get; set; }

    public IdWithTeam(int id)
    {
        this.id = id;
        this.team = new Team();
    }

    public IdWithTeam()
    {
    }
}

public class TeamWithMatches
{
    public Team Team { get; set; }
    public List<Match> Matches { get; set; }
}