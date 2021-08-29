using CommSec.Movie.Repository.Interfaces;
using MoviesLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommSec.Movie.Repository
{
	public class MovieRepository : IMovieRepository
	{
		readonly MovieDataSource _movieDataContext;

		public MovieRepository(MovieDataSource movieDataContext)
		{
			_movieDataContext = movieDataContext;
		}

		public async Task<int> AddMovieAsync(MovieData movie)
		{
			return await Task.Run(() => _movieDataContext.Create(movie));
		}

		public async Task<IList<MovieData>> GetAllAsync()
		{
			return await Task.Run(() => _movieDataContext.GetAllData());
		}

		public async Task<MovieData> GetMovieAsync(int id)
		{
			return await Task.Run(() => _movieDataContext.GetDataById(id));
		}

		public async Task UpdateMovieAsync(MovieData movie)
		{
			await Task.Run(() => _movieDataContext.Update(movie));
		}
	}
}
