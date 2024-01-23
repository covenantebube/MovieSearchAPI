using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace MovieSearchApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        private const string ApiKey = "4ebe41e8";

        private static List<string> SearchHistory = new List<string>();

        [HttpGet("search/{title}")]
        public async Task<IActionResult> Search(string title)
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync($"http://www.omdbapi.com/?apikey={ApiKey}&t={title}");
                var result = JsonConvert.DeserializeObject<dynamic>(response);

                // Check if "Title" property exists in the result
                if (result != null && result.Title != null)
                {
                    var searchResult = new
                    {
                        Title = (string)result.Title,
                        Year = (string)result.Year,
                        imdbID = (string)result.imdbID,
                        // Add other properties as needed
                    };

                    SearchHistory.Insert(0, title);
                    SearchHistory = SearchHistory.Take(5).ToList();

                    return Ok(new { SearchResult = searchResult, SearchHistory });
                }
                else
                {
                    return BadRequest(new { Message = "No search results found." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("details/{imdbId}")]
        public async Task<IActionResult> Details(string imdbId)
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync($"http://www.omdbapi.com/?apikey={ApiKey}&i={imdbId}");
                var result = JsonConvert.DeserializeObject<dynamic>(response);

                // Handle the "Ratings" property as JArray
                var ratingsArray = (JArray)result.Ratings;
                var ratings = ratingsArray != null
                    ? ratingsArray.Select(r => new { Source = (string)r["Source"], Value = (string)r["Value"] }).ToList<object>()
                    : new List<object>();

                // Create a custom object for detailed information
                var detailedInfo = new
                {
                    Title = (string)result.Title,
                    Year = (string)result.Year,
                    Rated = (string)result.Rated,
                    Released = (string)result.Released,
                    Runtime = (string)result.Runtime,
                    Genre = (string)result.Genre,
                    Director = (string)result.Director,
                    Writer = (string)result.Writer,
                    Actors = (string)result.Actors,
                    Plot = (string)result.Plot,
                    Language = (string)result.Language,
                    Country = (string)result.Country,
                    Awards = (string)result.Awards,
                    Poster = (string)result.Poster,
                    Ratings = ratings,
                    Metascore = (string)result.Metascore,
                    imdbRating = (string)result.imdbRating,
                    imdbVotes = (string)result.imdbVotes,
                    imdbID = (string)result.imdbID,
                    Type = (string)result.Type,
                    DVD = (string)result.DVD,
                    BoxOffice = (string)result.BoxOffice,
                    Production = (string)result.Production,
                    Website = (string)result.Website,
                    Response = (string)result.Response
                };

                return Ok(detailedInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

    }
}
