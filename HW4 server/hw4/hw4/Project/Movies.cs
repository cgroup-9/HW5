using System;
using System.Collections.Generic;

namespace hw4.Project
{
    public class Movies
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string PrimaryTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PrimaryImage { get; set; } = string.Empty;
        public int Year { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Language { get; set; } = string.Empty;
        public double Budget { get; set; }
        public double GrossWorldwide { get; set; }
        public string Genres { get; set; } = string.Empty;
        public bool IsAdult { get; set; }
        public int RuntimeMinutes { get; set; }
        public float AverageRating { get; set; }
        public int NumVotes { get; set; }
        public int? PriceToRent { get; set; }

        // Insert movie into database
        public bool Insert()
        {
            DBservices db = new DBservices();
            int result = db.InsertMovie(this);
            return result > 0;
        }

        // Read all movies from DB
        public static List<Movies> Read()
        {
            DBservices db = new DBservices();
            return db.ReadMovies();
        }

        // Get movies by title
        public static List<Movies> GetByTitle(string title)
        {
            DBservices db = new DBservices();
            return db.GetMoviesByTitle(title);
        }

        // Get movies by date range
        public static List<Movies> GetByReleaseDate(DateTime startDate, DateTime endDate)
        {
            DBservices db = new DBservices();
            return db.GetMoviesByReleaseDate(startDate, endDate);
        }

        // Delete movie by ID
        public static bool DeleteMovie(int id)
        {
            DBservices db = new DBservices();
            int result = db.DeleteMovieById(id);
            return result > 0;
        }
    }
}
