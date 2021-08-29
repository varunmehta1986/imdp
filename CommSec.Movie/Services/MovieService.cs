using CommSec.Movie.Repository.Interfaces;
using CommSec.Movie.Services.Interfaces;
using CommSec.Movie.Utils;
using Microsoft.Extensions.Caching.Memory;
using MoviesLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Transactions;

namespace CommSec.Movie.Services
{
	public class MovieService : IMovieService
	{
		private readonly IMovieRepository _movieRepository;
		private readonly IMemoryCache _memoryCache;
		public MovieService(IMovieRepository movieRepository, IMemoryCache memoryCache)
		{
			_movieRepository = movieRepository;
			_memoryCache = memoryCache;
		}

		public async Task<int> AddMovie(MovieData movieData)
		{
			using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled); 
			var newMovieId = await _movieRepository.AddMovieAsync(movieData); 

			//write to cache if its available, to make sure we get back newly added movies in subsequent requests
			if (_memoryCache.TryGetValue(CacheKeys.MovieList, out IList<MovieData> movieList))
			{
				movieData.MovieId = newMovieId;
				movieList.Add(movieData);
			}
			transactionScope.Complete();
			return newMovieId;
			
		}

		public async Task<IList<MovieData>> GetSearchedAndSortedMovies(string searchColumn = "", string searchKey = "", string sortAttribute = "", string sortOrder = "ASC") //ienumer?
		{
			
			var movieList = await GetSetCache();

			//search 
			if (!string.IsNullOrWhiteSpace(searchKey))
			{
				if (!string.IsNullOrWhiteSpace(searchColumn))
				{
					switch (searchColumn.ToLower())
					{
						case "title":
							movieList = movieList.Where(m => m.Title.Contains(searchKey,StringComparison.InvariantCultureIgnoreCase)).ToList();
							break;
						case "genre":
							movieList = movieList.Where(m => m.Genre.Contains(searchKey, StringComparison.InvariantCultureIgnoreCase)).ToList();
							break;
						case "releasedate":
							movieList = movieList.Where(m => m.ReleaseDate.ToString() == searchKey).ToList();
							break;
						case "classification":
							movieList = movieList.Where(m => m.Classification.Equals(searchKey,StringComparison.InvariantCultureIgnoreCase)).ToList();
							break;
						case "rating":
							movieList = movieList.Where(m => m.Rating.ToString() == searchKey).ToList();
							break;
						case "cast":
							movieList = movieList.Where(m => m.Cast.Any(c => c.Contains(searchKey, StringComparison.InvariantCultureIgnoreCase))).ToList();
							break;
					}
				}
				else
				{
					movieList = movieList.Where(m => m.Genre.Contains(searchKey, StringComparison.InvariantCultureIgnoreCase) 
												|| m.ReleaseDate.ToString() == searchKey 
												|| m.Title.Contains(searchKey, StringComparison.InvariantCultureIgnoreCase)
												|| m.Cast.Any(c => c.Contains(searchKey, StringComparison.InvariantCultureIgnoreCase))).ToList();
				}
			}
			//sort
			if (!string.IsNullOrWhiteSpace(sortAttribute))
			{
				movieList = movieList.AsQueryable().OrderBy(sortAttribute + " " + sortOrder).ToList();
			}
			return movieList;
			
		}
		public async Task<MovieData> GetMovieAsync(int movieId)
		{
			if (!_memoryCache.TryGetValue(CacheKeys.MovieList, out IList<MovieData> movieList))
			{
				//This is for the next requests
				var task = Task.Run(() =>
				{
					//fire and forget
					_ = GetSetCache();
				});

			}
			else
			{
				//if cache is found, get from cache.
				return movieList.Where(m => m.MovieId == movieId).FirstOrDefault();
			}
			//get is time consuming(100), so need to make sure we always get it from Cache next time onwards.
			return await _movieRepository.GetMovieAsync(movieId);
			
		}
		public async Task UpdateMovieAsync(MovieData movieData)
		{
			using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
			await _movieRepository.UpdateMovieAsync(movieData);
			//update to cache if cache is available, to make sure we get back updated movie in subsequent requests
			if (_memoryCache.TryGetValue(CacheKeys.MovieList, out IList<MovieData> movieList))
			{
				var movieToUpdate = movieList.Where(m => m.MovieId == movieData.MovieId).FirstOrDefault();
				movieToUpdate.Title = movieData.Title;
				movieToUpdate.Cast = movieData.Cast;
				movieToUpdate.Classification = movieData.Classification;
				movieToUpdate.Genre = movieData.Genre;
				movieToUpdate.Rating = movieData.Rating;
				movieToUpdate.ReleaseDate = movieData.ReleaseDate;
			}
			transactionScope.Complete();
		}

		private async Task<IList<MovieData>> GetSetCache()
		{
			//get movie list from cache or db and set it for subsequent requests
			if (!_memoryCache.TryGetValue(CacheKeys.MovieList, out IList<MovieData> movieList))
			{
				movieList = await _movieRepository.GetAllAsync();
				var cacheExpiryOptions = new MemoryCacheEntryOptions
				{
					AbsoluteExpiration = DateTime.Now.AddDays(1)
				};
				_memoryCache.Set(CacheKeys.MovieList, movieList, cacheExpiryOptions);
			}
			return movieList;
		}
	}
}
