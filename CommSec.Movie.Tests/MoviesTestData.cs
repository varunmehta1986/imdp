using MoviesLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommSec.Movie.Tests
{
	public static class MoviesTestData
	{
		public static readonly IList<MovieData> Movies = new List<MovieData>()
		{
			new MovieData()
			{
				MovieId = 1,
				Classification = "M15+",
				Genre = "Action Adventure",
				Rating = 3,
				ReleaseDate = 2013,
				Title = "Olympus Has Fallen",
				Cast = new string[]{ "Gerard Butler", "Dylan McDermott","Aaron Eckhart","Angela Basset"}
			},
			new MovieData()
			{
				MovieId = 2,
				Classification = "M",
				Genre = "Action Adventure",
				Rating = 3,
				ReleaseDate = 2013,
				Title = "Man of Steel",
				Cast = new string[]{ "Christopher Meloni","Diane Lane", "Laurence Fishburne", "Amy Adams"}
			},
			new MovieData()
			{
				MovieId = 3,
				Title = "The Hangover Part III",
				ReleaseDate = 2013,
				Genre = "Comedy",
				Classification = "M15+",
				Rating = 1,
				Cast = new string[]{ "Zach Galifianakis", "Melissa McCarthy", "Heather Graham","Ed Helms","Sasha Barrese"}
			},
			new MovieData()
			{
				MovieId = 4,
				Title = "The Bling Ring",
				ReleaseDate = 2014,
				Genre = "Drama",
				Classification = "M15+",
				Rating = 3,
				Cast = new string[]{"Varun Mehta"}
			},
			new MovieData()
			{
				MovieId = 5,
				Title = "Aftershock",
				ReleaseDate = 2012,
				Genre = "Horror",
				Classification = "R",
				Rating = 3,
				Cast = new string[]{"Cast A", "Cast B"}
			},
			new MovieData()
			{
				MovieId = 6,
				Title = "The Fly",
				ReleaseDate = 2016,
				Genre = "Science Fiction",
				Classification = "M15+",
				Rating = 4,
				Cast = new string[]{"Fly 1", "Fly 2"}
			},
			new MovieData()
			{
				MovieId = 7,
				Title = "The Nightmare Before Christmas",
				ReleaseDate = 1993,
				Genre = "Cartoon",
				Classification = "G",
				Rating = 4,
				Cast = new string[]{"Cartoon 1", "Cartoon 2"}
			},
			new MovieData()
			{
				MovieId = 8,
				Title = "Angels and Demons",
				ReleaseDate = 2009,
				Genre = "Thriller",
				Classification = "M15+",
				Rating = 4,
				Cast = new string[]{"Angel 1", "Angel 2", "Demons 1" , "Demons 2", "Varu M"}
			}
		};
	}
}
