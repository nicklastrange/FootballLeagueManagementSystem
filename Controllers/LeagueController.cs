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
        List<League> leaguesList = ctx.Leagues.Include(x => x.TeamList)
            .Include(x => x.Schedule)
            .Where(x => !x.IsFinished)
            .ToList();
        return View(leaguesList);
    }
    
    public IActionResult Archive()
    {
        List<League> leaguesList = ctx.Leagues.Include(x => x.TeamList)
            .Include(x => x.Schedule)
            .Where(x => x.IsFinished)
            .ToList();
        return View(leaguesList);
    }
    
    public IActionResult Table(int? id)
    {
        League league = ctx.Leagues.Include(x => x.Schedule).Include(x => x.TeamList).First(league => league.Id == id);

        List<TeamWithMatches> teamWithMatchesList = new List<TeamWithMatches>();
        foreach (var t in league.TeamList)
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
        League league = ctx.Leagues.Include(x => x.Schedule).Include(x => x.TeamList).First(league => league.Id == id);

        List<Match> schedule = new List<Match>();
        foreach (var team in league.TeamList)
        {
            List<Team> teamsWithoutCurrentTeam = new List<Team>(league.TeamList);
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
    
    public IActionResult Clone(int? id)
    {
        League league = ctx.Leagues.Include(x => x.Schedule).Include(x => x.TeamList).First(league => league.Id == id);
        
        var copyLeague = new League();
        var copyTeams = new List<Team>();
        league.TeamList.ForEach(x =>
        {
            Team newTeam = new Team();
            newTeam.Name = x.Name;
            newTeam.CreatedAt = x.CreatedAt;
            copyTeams.Add(newTeam);
        });
        copyLeague.Name = league.OriginalName;
        copyLeague.OriginalName = league.OriginalName;
        copyLeague.TeamList = copyTeams;
        ctx.Add(copyLeague);
        ctx.Update(league);
        ctx.SaveChanges();
        TempData["success"] = "League cloned successfully!";
        return RedirectToAction("Index");
    }

    public IActionResult Finish(int? id)
    {
        var finishedLeague = ctx.Leagues.Include(x => x.Schedule)
            .Include(x => x.TeamList)
            .First(x => x.Id == id);

        var suffix = DateTime.Now;
        finishedLeague.IsStarted = false;
        finishedLeague.IsFinished = true;
        finishedLeague.Name = finishedLeague.Name + " " + suffix;
        ctx.Update(finishedLeague);
        ctx.SaveChanges();
        TempData["success"] = "Season successfully finished!";
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

        league.OriginalName = league.Name;
        ctx.Leagues.Add(league);
        ctx.SaveChanges();
        TempData["success"] = "League created successfully!";
        return RedirectToAction("Index"); //we can add second parameter which defines controller of the action
    }
    
    // GET
    public IActionResult Edit(int? id)
    {
        var leagueFromDb = ctx.Leagues.Include(x => x.Schedule)
            .Include(x => x.TeamList)
            .First(x => x.Id == id);
        
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

        var dbLeague = ctx.Leagues.Include(x => x.Schedule)
            .Include(x => x.TeamList)
            .First(x => x.Id == league.Id);
        
        dbLeague.Name = league.Name;
        dbLeague.OriginalName = league.Name;
        for (int i = 0; i < league.TeamList.Count; i++)
        {
            dbLeague.TeamList[i].Name = league.TeamList[i].Name;
        }
        ctx.Leagues.Update(dbLeague);
        ctx.SaveChanges();
        TempData["success"] = "League updated successfully!";
        return RedirectToAction("Index"); //we can add second parameter which defines controller of the action
    }
    
    // GET
    public IActionResult Delete(int? id)
    {
        var leagueFromDb = ctx.Leagues.Include(x => x.TeamList)
            .Include(x => x.Schedule)
            .First(x => x.Id == id);
        
        return View(leagueFromDb);
    }
    
    // POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int? id)
    {
        var leagueFromDb = ctx.Leagues.Include(x => x.TeamList)
            .Include(x => x.Schedule)
            .First(x => x.Id == id);

        ctx.Leagues.Remove(leagueFromDb);
        leagueFromDb.TeamList.ForEach(x => ctx.Teams.Remove(x));
        leagueFromDb.Schedule.ForEach(x => ctx.Matches.Remove(x));
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

        league.TeamList.Add(idWithTeam.team);
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
    public int countOfWins { get; set; } = 0;
    public int countOfDraws { get; set; } = 0;
    public int countOfLoses { get; set; } = 0;
    public int countOfGoalsScored { get; set; } = 0;
    public int countOfGoalsConceded { get; set; } = 0;
    public int countOfPoints { get; set; } = 0;
    public int goalDifference { get; set; } = 0;
    public Team Team { get; set; }
    public List<Match> Matches { get; set; }
}