using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Backlogger.ApiModels
{
  public partial class TmdbMovieRoot
  {
    [JsonProperty("adult")]
    public bool Adult { get; set; }

    [JsonProperty("backdrop_path")]
    public string BackdropPath { get; set; }

    [JsonProperty("belongs_to_collection")]
    public object BelongsToCollection { get; set; }

    [JsonProperty("budget")]
    public long Budget { get; set; }

    [JsonProperty("genres")]
    public List<TmdbMovieGenre> Genres { get; set; }

    [JsonProperty("homepage")]
    public string Homepage { get; set; }

    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("imdb_id")]
    public string ImdbId { get; set; }

    [JsonProperty("original_language")]
    public string OriginalLanguage { get; set; }

    [JsonProperty("original_title")]
    public string OriginalTitle { get; set; }

    [JsonProperty("overview")]
    public string Overview { get; set; }

    [JsonProperty("popularity")]
    public double Popularity { get; set; }

    [JsonProperty("poster_path")]
    public string PosterPath { get; set; }

    [JsonProperty("production_companies")]
    public List<TmdbMovieProductionCompany> ProductionCompanies { get; set; }

    [JsonProperty("production_countries")]
    public List<TmdbMovieProductionCountry> ProductionCountries { get; set; }

    [JsonProperty("release_date")]
    public string ReleaseDate { get; set; }

    [JsonProperty("revenue")]
    public long Revenue { get; set; }

    [JsonProperty("runtime")]
    public long Runtime { get; set; }

    [JsonProperty("spoken_languages")]
    public List<TmdbMovieSpokenLanguage> SpokenLanguages { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("tagline")]
    public string Tagline { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("video")]
    public bool Video { get; set; }

    [JsonProperty("vote_average")]
    public double VoteAverage { get; set; }

    [JsonProperty("vote_count")]
    public long VoteCount { get; set; }
  }

  public partial class TmdbMovieGenre
  {
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
  }

  public partial class TmdbMovieProductionCompany
  {
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("logo_path")]
    public string LogoPath { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("origin_country")]
    public string OriginCountry { get; set; }
  }

  public partial class TmdbMovieProductionCountry
  {
    [JsonProperty("iso_3166_1")]
    public string Iso3166_1 { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
  }

  public partial class TmdbMovieSpokenLanguage
  {
    [JsonProperty("iso_639_1")]
    public string Iso639_1 { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
  }
}