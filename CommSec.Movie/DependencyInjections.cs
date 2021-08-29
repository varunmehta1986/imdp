using CommSec.Movie.Repository;
using CommSec.Movie.Repository.Interfaces;
using CommSec.Movie.Services;
using CommSec.Movie.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using MoviesLibrary;

namespace CommSec.Movie
{
	public class DependencyInjections
	{
		public static void Inject(IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton<MovieDataSource>()
								.AddTransient<IMovieRepository, MovieRepository>()
								.AddTransient<IMovieService, MovieService>();

		}
	}
}
