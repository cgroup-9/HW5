using hw4.Project;

public class Movies
{
    public int? Id { get; set; }
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

    public int Insert()
    {
        DBservices db = new DBservices();
        return db.InsertMovie(this);
    }

    public static List<Movies> Read()
    {
        DBservices db = new DBservices();
        return db.ReadMovies();
    }

    public static (List<Movies>, int) ReadPagedWithTotal(int page, int pageSize)
    {
        DBservices db = new();
        return db.ReadPagedMoviesWithTotal(page, pageSize);
    }

    public static List<Movies> GetByTitle(string title)
    {
        DBservices db = new DBservices();
        return db.GetMoviesByTitle(title);
    }

    public static List<Movies> GetByReleaseDate(DateTime startDate, DateTime endDate)
    {
        DBservices db = new DBservices();
        return db.GetMoviesByReleaseDate(startDate, endDate);
    }

    public static int DeleteRegularMovie(int movieId)
    {
        DBservices db = new DBservices();
        return db.DeleteMovieById(movieId);
    }

    public static int DeleteRentedMovie(int userId, int movieId)
    {
        DBservices db = new DBservices();
        return db.DeleteRentedMovieById(userId, movieId);
    }
}
