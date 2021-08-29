using MoviesLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommSec.Movie.Repository.Interfaces
{
	public interface IMovieRepository
	{
		Task<IList<MovieData>> GetAllAsync();
		Task<MovieData> GetMovieAsync(int id);
		Task<int> AddMovieAsync(MovieData movieData);
		Task UpdateMovieAsync(MovieData movieData);


	}
}
