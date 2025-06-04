using hw4.Project;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace hw4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        // GET: api/Movies
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<Movies> movies = Movies.Read();
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        // GET: api/Movies/paged?page=1&pageSize=20
        [HttpGet("paged")]
        public IActionResult GetPaged([FromQuery] int page, [FromQuery] int pageSize)
        {
            try
            {
                var (movies, totalPages) = Movies.ReadPagedWithTotal(page, pageSize);
                return Ok(new { movies, totalPages });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }


        // GET api/Movies/search?title=...
        [HttpGet("search")]
        public IActionResult GetByTitle(string title)
        {
            try
            {
                List<Movies> result = Movies.GetByTitle(title);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        // GET api/Movies/searchByPath/startDate/yyyy-mm-dd/endDate/yyyy-mm-dd
        [HttpGet("searchByPath/startDate/{startDate}/endDate/{endDate}")]
        public IActionResult GetByReleaseDate(DateTime startDate, DateTime endDate)
        {
            try
            {
                List<Movies> result = Movies.GetByReleaseDate(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        // POST api/Movies
        [HttpPost]
        public IActionResult Post([FromBody] Movies movie)
        {
            try
            {
                int result = movie.Insert();
                if (result == 0)
                    return Ok(new { success = true, message = "Movie inserted successfully." });
                else if (result == 3)
                    return BadRequest("A movie with this title already exists.");
                else
                    return StatusCode(500, "Failed to insert movie.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        [HttpPost("bulk")]
        public IActionResult InsertBulkMovies([FromBody] List<Movies> movies)
        {
            int insertedCount = 0;
            List<string> duplicateTitles = new();
            List<string> failedTitles = new();

            foreach (var movie in movies)
            {
                try
                {
                    int result = movie.Insert();

                    if (result == 0)
                        insertedCount++;
                    else if (result == 3)
                        duplicateTitles.Add(movie.PrimaryTitle);
                    else
                        failedTitles.Add(movie.PrimaryTitle);
                }
                catch (Exception ex)
                {
                    failedTitles.Add(movie.PrimaryTitle + $" (Error: {ex.Message})");
                }
            }

            return Ok(new
            {
                Inserted = insertedCount,
                Duplicates = duplicateTitles,
                Failed = failedTitles,
                Total = movies.Count
            });
        }

        // POST api/Movies/rent
        [HttpPost("rent")]
        public IActionResult RentMovie([FromBody] RentedMovie rent)
        {
            try
            {
                if (rent.RentStart >= rent.RentEnd)
                {
                    return BadRequest("⛔ End date must be after start date.");
                }

                int result = rent.Rent();

                if (result > 0)
                    return Ok(new { message = "🎬 Rental completed successfully." });
                else
                    return BadRequest(new { message = "❌ Rental failed: Logic prevented execution or duplicate rental." });
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    return BadRequest(new { message = "❗ This movie has already been rented by this user for the selected start date." });

                return StatusCode(500, $"❌ SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Server error: {ex.Message}");
            }
        }

        // GET api/Movies/rented/{userId}
        [HttpGet("rented/{userId}")]
        public IActionResult GetRentedMoviesByUserId(int userId)
        {
            try
            {
                var rentedMovies = DBservices.GetRentedMoviesByUserId(userId);
                return Ok(rentedMovies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        // DELETE api/Movies/delete-regular/{id}
        [HttpDelete("delete-regular/{id}")]
        public IActionResult DeleteRegularMovie(int id)
        {
            try
            {
                int result = Movies.DeleteRegularMovie(id);

                if (result > 0)
                    return Ok("✅ Regular movie deleted successfully.");
                else
                    return NotFound("❌ Movie not found or already deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        // DELETE api/Movies/delete-rented/{userId}/{movieId}
        [HttpDelete("delete-rented/{userId}/{movieId}")]
        public IActionResult DeleteRentedMovie(int userId, int movieId)
        {
            try
            {
                int result = Movies.DeleteRentedMovie(userId, movieId);

                if (result > 0)
                    return Ok("✅ Rented movie deleted successfully.");
                else
                    return NotFound("❌ Rented movie not found or already deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Internal server error: {ex.Message}");
            }
        }
        [HttpPost("forward-rented")]
        public IActionResult ForwardRentedMovie([FromBody] ForwardRentalRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.ToUserName))
                    return BadRequest(new { message = "Recipient username is required." });

                RentedMovie rent = new RentedMovie
                {
                    UserId = request.FromUserId,
                    MovieId = request.MovieId
                };

                int result = rent.ForwardToAnotherUser(request.ToUserName);

                return result switch
                {
                    1 => Ok(new { message = "✅ Movie forwarded successfully." }),
                    -1 => BadRequest(new { message = "❌ Target user not found." }),
                    -2 => BadRequest(new { message = "⛔ Movie already rented by target user during this time." }),
                    -3 => BadRequest(new { message = "⚠️ Target user already has this exact rental." }),
                    _ => StatusCode(500, new { message = "❌ Unknown error during movie forwarding." })
                };

            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = $"❌ SQL Error: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"❌ Server error: {ex.Message}" });
            }
        }



    }
}
