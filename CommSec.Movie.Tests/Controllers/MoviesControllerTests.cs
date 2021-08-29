using CommSec.Movie.Controllers;
using CommSec.Movie.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Moq;
using MoviesLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CommSec.Movie.Tests.Controllers
{
	public class MoviesControllerTests
	{
		private readonly MoviesController _moviesController;
		private Mock<IMovieService> _mockMovieService;
		public MoviesControllerTests()
		{
			_mockMovieService = new Mock<IMovieService>();
			_moviesController = new MoviesController(_mockMovieService.Object);
		}

		[Theory]
		[InlineData("title", "", "","","searchKey cannot be blank if searchColumn is supplied")]
		[InlineData("", "dd", "","","searchKey is too small to search")]
		[InlineData("abcd", "","","","Invalid search column name supplied")]
		[InlineData("","","cast","asc", "Sort by cast is not available at this moment")]
		[InlineData("", "", "title", "besc", "Invalid Sort Order")]
		public async Task GetTests_BadRequests(string searchColumn, string searchKey, string sortBy, string sortOrder, string errorMessage)
		{
			//arrange

			//act
			var result = await _moviesController.GetAsync(searchColumn, searchKey,sortBy,sortOrder);

			//assert
			Assert.NotNull(result);
			Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal(errorMessage, (result as BadRequestObjectResult).Value);
		}

		[Fact]
		public async Task GetTests_OkRequest()
		{
			//arrange
			_mockMovieService.Setup(m => m.GetSearchedAndSortedMovies(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
								.ReturnsAsync(new List<MovieData>());

			//act
			var result = await _moviesController.GetAsync(string.Empty, string.Empty);

			//assert
			Assert.NotNull(result);
			Assert.IsType<OkObjectResult>(result);
		}

		[Fact]
		public async Task GetTests_InternalServerError()
		{
			//arrange
			_mockMovieService.Setup(m => m.GetSearchedAndSortedMovies(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
								.Throws(new Exception());

			//act
			var result = await _moviesController.GetAsync(string.Empty, string.Empty);

			//assert
			Assert.NotNull(result);
			Assert.IsType<ObjectResult>(result);
			Assert.Equal(500, (result as ObjectResult).StatusCode);
		}

		[Fact]
		public async Task MoviesGetApiTest_BadRequest()
		{
			//arrange
			using var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
			var client = server.CreateClient();

			var request = new HttpRequestMessage(HttpMethod.Get, $"movies?searchColumn=title");

			//act
			var rawResponse = await client.SendAsync(request);
			var response = await rawResponse.Content.ReadAsStringAsync();

			//assert
			Assert.Equal(HttpStatusCode.BadRequest, rawResponse.StatusCode);
			Assert.Equal("searchKey cannot be blank if searchColumn is supplied", response);
		}

		[Fact]
		public async Task PostTest_OkResult()
		{
			//arrange
			_mockMovieService.Setup(m => m.AddMovie(It.IsAny<MovieData>())).ReturnsAsync(10);

			//act
			var result = await _moviesController.PostAsync(new MovieData());

			//assert
			Assert.NotNull(result);
			Assert.IsType<OkObjectResult>(result);
			Assert.Equal(10, (result as OkObjectResult).Value);
		}
		[Fact]
		public async Task PostTest_Error()
		{
			//arrange
			_mockMovieService.Setup(m => m.AddMovie(It.IsAny<MovieData>())).Throws(new Exception());

			//act
			var result = await _moviesController.PostAsync(new MovieData());

			//assert
			Assert.NotNull(result);
			Assert.IsType<ObjectResult>(result);
			Assert.Equal((int)HttpStatusCode.InternalServerError, (result as ObjectResult).StatusCode);
		}

		[Fact]
		public async Task PutTest_OkResult()
		{
			//arrange
			_mockMovieService.Setup(m => m.GetMovieAsync(It.IsAny<int>())).ReturnsAsync(new MovieData()
			{
				MovieId = 20,
				Title = "Put test movie"
			});


			//act
			var result = await _moviesController.PutAsync(new MovieData() { MovieId = 20 });

			//assert
			Assert.NotNull(result);
			Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Movie updated", (result as OkObjectResult).Value);
		}

		[Fact]
		public async Task PutTest_BadRequestResult()
		{
			//arrange
			_mockMovieService.Setup(m => m.GetMovieAsync(It.IsAny<int>()));


			//act
			var result = await _moviesController.PutAsync(new MovieData() { MovieId = 20 });

			//assert
			Assert.NotNull(result);
			Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal("Invalid Movie", (result as NotFoundObjectResult).Value);
		}

		[Fact]
		public async Task PutTest_OnGetMovie_InternalServerError()
		{
			//arrange
			_mockMovieService.Setup(m => m.GetMovieAsync(It.IsAny<int>())).ThrowsAsync(new Exception());


			//act
			var result = await _moviesController.PutAsync(new MovieData() { MovieId = 20 });

			//assert
			Assert.NotNull(result);
			Assert.IsType<ObjectResult>(result);
			Assert.Equal((int)HttpStatusCode.InternalServerError, (result as ObjectResult).StatusCode);
		}

		[Fact]
		public async Task PutTest_OnUpdate_InternalServerError()
		{
			//arrange

			//get works
			_mockMovieService.Setup(m => m.GetMovieAsync(It.IsAny<int>())).ReturnsAsync(new MovieData()
			{
				MovieId = 20,
				Title = "Put test movie"
			});
			//but update fails
			_mockMovieService.Setup(m => m.UpdateMovieAsync(It.IsAny<MovieData>())).ThrowsAsync(new Exception());


			//act
			var result = await _moviesController.PutAsync(new MovieData() { MovieId = 20 });

			//assert
			Assert.NotNull(result);
			Assert.IsType<ObjectResult>(result);
			Assert.Equal((int)HttpStatusCode.InternalServerError, (result as ObjectResult).StatusCode);
		}
		[Fact]
		public async Task GetTest_OkResult()
		{
			//arrange
			_mockMovieService.Setup(m => m.GetMovieAsync(It.IsAny<int>())).ReturnsAsync(new MovieData()
			{
				MovieId = 20,
				Title = "Put test movie"
			});


			//act
			var result = await _moviesController.GetMovieAsync(20);

			//assert
			Assert.NotNull(result);
			Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Put test movie", ((result as OkObjectResult).Value as MovieData).Title);
		}

		[Fact]
		public async Task GetTest_NotFoundResult()
		{
			//arrange
			_mockMovieService.Setup(m => m.GetMovieAsync(It.IsAny<int>()));

			//act
			var result = await _moviesController.GetMovieAsync(20);

			//assert
			Assert.NotNull(result);
			Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal("No movie found for that id.", (result as NotFoundObjectResult).Value);
		}
		[Fact]
		public async Task GetTest_InternalServerError()
		{
			//arrange
			_mockMovieService.Setup(m => m.GetMovieAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

			//act
			var result = await _moviesController.GetMovieAsync(20);

			//assert
			Assert.NotNull(result);
			Assert.IsType<ObjectResult>(result);
			Assert.Equal((int)HttpStatusCode.InternalServerError, (result as ObjectResult).StatusCode);
		}
	}
}