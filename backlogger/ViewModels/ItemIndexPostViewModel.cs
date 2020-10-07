using Backlogger.ApiModels;

namespace Backlogger.ViewModels
{
  public class ItemIndexPostViewModel
  {
    public RawgSearchRoot GamesSearch { get; set; } = null;
    public TmdbMovieSearchRoot MovieSearch { get; set; } = null;
    public TmdbTvSearchRoot TvSearch { get; set; } = null;
    public string SearchOption { get; set; }
    public string SearchString { get; set; }
    public long Results { get; set; }
    public long Pages { get; set; } = 1;
    public int CurrentPage { get; set; }
  }
}