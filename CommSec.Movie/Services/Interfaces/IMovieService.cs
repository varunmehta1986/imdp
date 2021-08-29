using MoviesLibrary;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommSec.Movie.Services.Interfaces
{
	public interface IMovieService
	{
		Task<IList<MovieData>> GetSearchedAndSortedMovies(string searchColumn, string searchKey, string sortAttribute, string sortOrder);
		Task<int> AddMovie(MovieData movieData);
		Task UpdateMovieAsync(MovieData movieData);
		Task<MovieData> GetMovieAsync(int movieId);

	}
}
