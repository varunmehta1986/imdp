using CommSec.Movie.Repository.Interfaces;
using CommSec.Movie.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MoviesLibrary;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CommSec.Movie.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class MoviesController : ControllerBase
	{
		private readonly IMovieService _movieService;

		public MoviesController(IMovieService movieRepository)
		{
			_movieService = movieRepository;
		}

		//POST : /movies
		[HttpPost]
		public async Task<IActionResult> PostAsync(MovieData movieData)
		{
			try
			{
				return new OkObjectResult(await _movieService.AddMovie(movieData));
			}
			catch (Exception exc)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, exc);
			}
		}

		//GET: /movies/
		[HttpGet]
		[Route("{movieId}")]
		public async Task<IActionResult> GetMovieAsync(int movieId)
		{
			try
			{
				var dbMovie = await _movieService.GetMovieAsync(movieId);
				if (dbMovie == null)
				{
					return new NotFoundObjectResult("No movie found for that id.");
				}
				return new OkObjectResult(dbMovie);
			}
			catch (Exception exc)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, exc);
			}
		}

		//PUT: /movies/
		[HttpPut]
		public async Task<IActionResult> PutAsync(MovieData movieData)
		{
			try
			{
				var dbMovie = await _movieService.GetMovieAsync(movieData.MovieId);
				if (dbMovie == null)
				{
					return new NotFoundObjectResult("Invalid Movie");
				}
				else
				{
					await _movieService.UpdateMovieAsync(movieData);
					return new OkObjectResult("Movie updated");
				}
			}
			catch (Exception exc)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, exc);
			}

		}

		[HttpGet]
		public async Task<IActionResult> GetAsync([FromQuery] string searchColumn, [FromQuery] string searchKey = "",
															[FromQuery] string sortBy = "", [FromQuery] string sortOrder = "ASC")
		{
			if (!string.IsNullOrWhiteSpace(searchColumn))
			{
				bool isValidColumn = false;
				//validate the column name is correct
				foreach (var property in typeof(MovieData).GetProperties())
				{
					if (property.Name.Equals(searchColumn, StringComparison.InvariantCultureIgnoreCase))
					{
						isValidColumn = true;
						break;
					}
				}
				if (!isValidColumn)
				{
					return new BadRequestObjectResult("Invalid search column name supplied");
				}
			}
			if (!string.IsNullOrWhiteSpace(searchColumn) && string.IsNullOrWhiteSpace(searchKey))
			{
				return new BadRequestObjectResult("searchKey cannot be blank if searchColumn is supplied");
			}
			if (string.IsNullOrWhiteSpace(searchColumn) && !string.IsNullOrWhiteSpace(searchKey) && searchKey.Length < 4)
			{
				return new BadRequestObjectResult("searchKey is too small to search");
			}
			if (sortOrder.ToLower() != "asc" && sortOrder.ToLower() != "desc")
			{
				return new BadRequestObjectResult("Invalid Sort Order");
			}
			if (sortBy.ToLower() == "cast")
			{
				return new BadRequestObjectResult("Sort by cast is not available at this moment");
			}
			try
			{
				return new OkObjectResult(await _movieService.GetSearchedAndSortedMovies(searchColumn, searchKey, sortBy, sortOrder));
			}
			catch (Exception exc)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, exc);
			}
		}
	}
}
