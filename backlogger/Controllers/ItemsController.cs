using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Backlogger.Models;
using Backlogger.ViewModels;
using Backlogger.ApiModels;

namespace Backlogger.Controllers
{
  public class ItemsController : Controller
  {
    private readonly BackloggerContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public ItemsController(BackloggerContext db, UserManager<ApplicationUser> userManager)
    {
      _db = db;
      _userManager = userManager;
    } 
    public IActionResult Index(string typeFilter, bool showCompleted = false)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      List<ItemUser> joinList = _db.ItemUser.Where(join => join.UserId == userId).Include(join => join.Item).ToList();
      List<Item> userItems = new List<Item>();
      foreach(ItemUser join in joinList)
      {
        userItems.Add(join.Item);
      }
      ItemIndexViewModel model = new ItemIndexViewModel();
      List<Item> orderedUserItems = userItems.OrderBy(item => item.Priority).ToList();
      model.ItemList = orderedUserItems;
      if (showCompleted == false)
      {
        List<Item> uncompletedItems = model.ItemList.Where(item => item.Watched == false).ToList();
        model.ItemList = uncompletedItems;
      }
      else
      {
        List<Item> completedItems = model.ItemList.Where(item => item.Watched == true).ToList();
        model.ItemList = completedItems;
        model.ShowingCompleted = true;
      }
      if (typeFilter != null)
      {
        List<Item> filteredItems = model.ItemList.Where(item => item.Type == typeFilter).ToList();
        model.ItemList = filteredItems;
        model.TypeFilter = typeFilter;
      }
      return View(model);
    }

    [HttpPost]
    public IActionResult Index (string searchOption = null, string searchString = null, int page = 1)
    {
      if (String.IsNullOrEmpty(searchString))
      {
        return RedirectToAction("Index", "Home");
      }
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      List<ItemUser> joinList = _db.ItemUser.Where(join => join.UserId == userId).Include(join => join.Item).ToList();
      List<long> userApiIds = new List<long>();
      foreach(ItemUser join in joinList)
      {
        userApiIds.Add(join.Item.ApiId);
      }
      ItemIndexViewModel model = new ItemIndexViewModel();
      model.ApiIds = userApiIds;
      model.CurrentPage = page;
      model.SearchOption = searchOption;
      model.SearchString = searchString;
      if (searchOption == "games")
      {
        RawgSearchRoot results = Rawg.GetGamesSearch(searchString, page);
        model.GamesSearch = results;
        model.Results = results.Count;
        model.Pages = (results.Count + 19) / 20;
      }
      else if (searchOption == "movies")
      {
        TmdbMovieSearchRoot results = Tmdb.GetMoviesSearch(searchString, page);
        model.MovieSearch = results;
        model.Results = results.TotalResults;
        model.Pages = results.TotalPages;
      }
      else if (searchOption == "tv")
      {
        TmdbTvSearchRoot results = Tmdb.GetTvSearch(searchString, page);
        model.TvSearch = results;
        model.Results = results.TotalResults;
        model.Pages = results.TotalPages;
      }
      return View(model);
    }

    public IActionResult Details(long id, string type, string screenshot = null)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      List<ItemUser> joinList = _db.ItemUser.Where(join => join.UserId == userId).Include(join => join.Item).ToList();
      List<long> userApiIds = new List<long>();
      foreach(ItemUser join in joinList)
      {
        userApiIds.Add(join.Item.ApiId);
      }
      ItemDetailsViewModel model = new ItemDetailsViewModel();
      model.ApiIds = userApiIds;
      if (type == "game")
      {
        RawgIdRoot result = Rawg.GetGameById(id);
        model.GameDetails = result;
        model.ScreenShot = screenshot;
      }
      else if (type == "movie")
      {
        TmdbMovieRoot result = Tmdb.GetMovieById(id);
        model.MovieDetails = result;
      }
      else if (type == "tv")
      {
        TmdbTvRoot result = Tmdb.GetTvById(id);
        model.TvDetails = result;
      }
      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(long id, string type, string screenshot)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      Item newItem = new Item();
      if (type == "game")
      {
        RawgIdRoot itemDetails = Rawg.GetGameById(id);
        newItem.GetRawgValues(itemDetails);
        newItem.Poster = screenshot;
        newItem.Priority = currentUser.PriorityValue;
        _db.Items.Add(newItem);
        ItemUser newItemUser = new ItemUser{Item = newItem, User = currentUser, ItemId = newItem.ItemId, UserId = userId};
        _db.ItemUser.Add(newItemUser);
        currentUser.PriorityValue++;
        var result = await _userManager.UpdateAsync(currentUser);
        _db.SaveChanges();
      }
      else if (type == "movie")
      {
        TmdbMovieRoot itemDetails = Tmdb.GetMovieById(id);
        newItem.GetTmdbMovieValues(itemDetails);
        newItem.Priority = currentUser.PriorityValue;
        _db.Items.Add(newItem);
        ItemUser newItemUser = new ItemUser{Item = newItem, User = currentUser, ItemId = newItem.ItemId, UserId = userId};
        _db.ItemUser.Add(newItemUser);
        currentUser.PriorityValue++;
        var result = await _userManager.UpdateAsync(currentUser);
        _db.SaveChanges();
      }
      else if (type == "tv")
      {
        TmdbTvRoot itemDetails = Tmdb.GetTvById(id);
        newItem.GetTmdbTvValues(itemDetails);
        newItem.Priority = currentUser.PriorityValue;
        _db.Items.Add(newItem);
        ItemUser newItemUser = new ItemUser{Item = newItem, User = currentUser, ItemId = newItem.ItemId, UserId = userId};
        _db.ItemUser.Add(newItemUser);
        currentUser.PriorityValue++;
        var result = await _userManager.UpdateAsync(currentUser);
        _db.SaveChanges();
      }
      return RedirectToAction("Index", new {typeFilter = type});
    }

    [HttpPost]
    public IActionResult SetPriority(long id1, long id2, string typeFilter)
    {
      if (id1 != -1 && id2 != -1)
      {
        Item firstItem = _db.Items.FirstOrDefault(item => item.ItemId == id1);
        Item secondItem = _db.Items.FirstOrDefault(item => item.ItemId == id2);
        int temp = firstItem.Priority;
        firstItem.Priority = secondItem.Priority;
        secondItem.Priority = temp;
        _db.Entry(firstItem).State = EntityState.Modified;
        _db.Entry(secondItem).State = EntityState.Modified;
        _db.SaveChanges();
      }
      return RedirectToAction("Index", new {typeFilter = typeFilter});
    }

    [HttpPost]
    public IActionResult SetWatched(int id, string typeFilter)
    {
      Item itemToSet = _db.Items.FirstOrDefault(item => item.ItemId == id);
      itemToSet.Watched = !itemToSet.Watched;
      _db.Entry(itemToSet).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index", new {typeFilter = typeFilter});
    }

    [HttpPost]
    public IActionResult Delete(int id, string typeFilter)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      Item itemToDelete = _db.Items.FirstOrDefault(item => item.ItemId == id);
      ItemUser joinToDelete = _db.ItemUser.FirstOrDefault(join => join.UserId == userId && join.ItemId == id);
      _db.Items.Remove(itemToDelete);
      _db.ItemUser.Remove(joinToDelete);
      _db.SaveChanges();
      return RedirectToAction("Index", new {typeFilter = typeFilter});
    }
  }
}
