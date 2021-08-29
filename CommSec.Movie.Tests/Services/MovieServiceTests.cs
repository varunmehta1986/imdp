using CommSec.Movie.Repository.Interfaces;
using CommSec.Movie.Services;
using CommSec.Movie.Utils;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using MoviesLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CommSec.Movie.Tests.Services
{
	public class MovieServiceTests : IDisposable
	{
		readonly Mock<IMovieRepository> _mockMovieRepository = new();
		private readonly IList<MovieData> Movies = MoviesTestData.Movies;
		readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
		public MovieServiceTests()
		{
			_mockMovieRepository.Setup(m => m.GetAllAsync()).ReturnsAsync(Movies);
		}
		public void Dispose()
		{
			_memoryCache.Remove(CacheKeys.MovieList);
		}
		[Fact]
		public void GetSearchedAndSortedMoviesTests_NoCache_GetAll()
		{
			//arrange
			var movieService = new MovieService(_mockMovieRepository.Object, _memoryCache);

			//act
			var result = movieService.GetSearchedAndSortedMovies(string.Empty, string.Empty, string.Empty).Result;

			//assert
			Assert.NotNull(result);
			Assert.Equal(Movies.Count, result.Count);

		}
		[Fact]
		public void GetSearchedAndSortedMoviesTests_FromCache_GetAll()
		{
			//arrange
			_memoryCache.Set(CacheKeys.MovieList, Movies);
			
			var movieService = new MovieService(_mockMovieRepository.Object, _memoryCache);

			//act
			var result = movieService.GetSearchedAndSortedMovies(string.Empty, string.Empty, string.Empty).Result;

			//assert
			Assert.NotNull(result);
			Assert.Equal(Movies.Count, result.Count);
		}

		[Theory]
		[InlineData("asc", "Aftershock", "The Nightmare Before Christmas")]
		[InlineData("desc", "The Nightmare Before Christmas", "Aftershock")]
		public void GetSearchedAndSortedMoviesTests_SortByTitle(string sortOrder, string firstTitle, string lastTitle)
		{
			//arrange
			var memoryCache = new MemoryCache(new MemoryCacheOptions());
			memoryCache.Set(CacheKeys.MovieList, Movies);
			var movieService = new MovieService(_mockMovieRepository.Object, _memoryCache);

			//act
			var result = movieService.GetSearchedAndSortedMovies(string.Empty, string.Empty, "title", sortOrder).Result;

			//assert
			Assert.NotNull(result);
			Assert.Equal(Movies.Count, result.Count);
			Assert.Equal(firstTitle, result[0].Title);
			Assert.Equal(lastTitle, result[7].Title);
		}

		[Theory]
		[InlineData( "asc", "Action Adventure", "Thriller")]
		[InlineData( "desc", "Thriller", "Action Adventure")]
		public void GetSearchedAndSortedMoviesTests_SortByGenre(string sortOrder, string firstGenre, string lastGenre)
		{
			//arrange
			_memoryCache.Set(CacheKeys.MovieList, Movies);
			var movieService = new MovieService(_mockMovieRepository.Object, _memoryCache);

			//act
			var result = movieService.GetSearchedAndSortedMovies(string.Empty, string.Empty, "genre", sortOrder).Result;

			//assert
			Assert.NotNull(result);
			Assert.Equal(Movies.Count, result.Count);
			Assert.Equal(firstGenre, result[0].Genre);
			Assert.Equal(lastGenre, result[7].Genre);
		}

		[Theory]
		[InlineData("asc", 1993,2016)]
		[InlineData("desc", 2016, 1993)]
		public void GetSearchedAndSortedMoviesTests_SortByReleaseDate(string sortOrder, int firstDate, int lastDate)
		{
			//arrange
			_memoryCache.Set(CacheKeys.MovieList, Movies);
			var movieService = new MovieService(_mockMovieRepository.Object, _memoryCache);

			//act
			var result =  movieService.GetSearchedAndSortedMovies(string.Empty,string.Empty, "releaseDate", sortOrder).Result;

			//assert
			Assert.NotNull(result);
			Assert.Equal(Movies.Count, result.Count);
			Assert.Equal(firstDate, result[0].ReleaseDate);
			Assert.Equal(lastDate, result[7].ReleaseDate);
		}

		[Theory]
		[InlineData("Genre","Thrill", 1)]
		[InlineData("Genre","Science",1)]
		[InlineData("Genre","Action Adventure", 2)]
		[InlineData("ReleaseDate","2013",3)]
		[InlineData("Cast","Varu",2)]
		[InlineData("Title","After",1)]
		[InlineData("Rating", "4", 3)]
		[InlineData("Classification","R",1)]
		public void GetSearchedAndSortedMoviesTests_SearchSingleColumn(string searchColumn,string searchKey, int resultCount)
		{
			//arrange
			_memoryCache.Set(CacheKeys.MovieList, Movies);
			var movieService = new MovieService(_mockMovieRepository.Object, _memoryCache);

			//act
			var result = movieService.GetSearchedAndSortedMovies(searchColumn, searchKey).Result;

			//assert
			Assert.NotNull(result);
			Assert.Equal(resultCount, result.Count); 
		}

		[Theory]
		[InlineData("Thrill", 1)]
		[InlineData("Science", 1)]
		[InlineData("Action Adventure", 2)]
		[InlineData("2013", 3)]
		[InlineData("Varu", 2)]
		[InlineData("After", 1)]
		public void GetSearchedAndSortedMoviesTests_SearchAllColumns(string searchKey, int resultCount)
		{
			//arrange
			_memoryCache.Set(CacheKeys.MovieList, Movies);
			var movieService = new MovieService(_mockMovieRepository.Object, _memoryCache);

			//act
			var result = movieService.GetSearchedAndSortedMovies(string.Empty, searchKey).Result;
		
			//assert
			Assert.NotNull(result);
			Assert.Equal(resultCount, result.Count);
		}

		[Fact]
		public void AddMovie_WithCache()
		{
			//arrange 
			_memoryCache.Set(CacheKeys.MovieList, Movies);
			var nextId = Movies.Count + 1;
			var movieService = new MovieService(_mockMovieRepository.Object, _memoryCache);
			_mockMovieRepository.Setup(m => m.AddMovieAsync(It.IsAny<MovieData>())).ReturnsAsync(nextId);

			//act
			var result = movieService.AddMovie(new MovieData()
			{
				Title = "Test Title",
				Cast = new string[] { "Cast A", "Cast B" },
				Classification = "G",
				Genre = "Thriller",
				Rating = 4,
				ReleaseDate = 2020
			}).Result;
			//assert
			Assert.Equal(nextId, result);

		}
		[Fact]
		public void AddMovie_NoCache()
		{
			//arrange
			var nextId = Movies.Count + 1;
			var movieService = new MovieService(_mockMovieRepository.Object, _memoryCache);
			_mockMovieRepository.Setup(m => m.AddMovieAsync(It.IsAny<MovieData>())).ReturnsAsync(nextId);

			//act
			var result = movieService.AddMovie(new MovieData()
			{
				Title = "Test Title",
				Cast = new string[] { "Cast A", "Cast B" },
				Classification = "G",
				Genre = "Thriller",
				Rating = 4,
				ReleaseDate = 2020
			}).Result;

			//assert
			Assert.Equal(nextId, result);
		}

		[Fact]
		public void GetMovie_NoCacheFirstRequest_FromCacheSecondRequest()
		{
			//arrange
			var movieService = new MovieService(_mockMovieRepository.Object, _memoryCache);
			int movieId = 2;
			var mainRepoTitle = "From Main Repo";
			_mockMovieRepository.Setup(m => m.GetMovieAsync(movieId)).ReturnsAsync(new MovieData() { MovieId = movieId, Title = mainRepoTitle });

			//act
			var movieFromRepo = movieService.GetMovieAsync(2).Result;
			//Let cache thread finish its job
			Thread.Sleep(2000);
			var movieFromCache = movieService.GetMovieAsync(2).Result;

			//assert
			Assert.NotNull(movieFromRepo);
			Assert.Equal(movieId, movieFromRepo.MovieId);
			Assert.Equal(mainRepoTitle, movieFromRepo.Title);
			Assert.Equal(movieFromCache.Title, Movies.Where(m=>m.MovieId == movieId).FirstOrDefault().Title);
		}

		[Fact]
		public void UpdateMovie_WithCache()
		{
			//arrange
			var movieService = new MovieService(_mockMovieRepository.Object, _memoryCache);
			_mockMovieRepository.Setup(m=>m.UpdateMovieAsync(It.IsAny<MovieData>()));
			_memoryCache.Set(CacheKeys.MovieList, Movies);
			var movieToUpdate = Movies[3];
			movieToUpdate.Title = "Updated to New Title";
			movieToUpdate.Rating = 10;

			//act
			movieService.UpdateMovieAsync(movieToUpdate).Wait();

			//assert
			var updatedMovieInCache = (_memoryCache.Get(CacheKeys.MovieList) as IList<MovieData>)[3];
			Assert.Equal(movieToUpdate.Title, updatedMovieInCache.Title);
		}
	}
}
