using hw4.Project;
using Microsoft.AspNetCore.Mvc;

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

        // GET api/Movies/{id}
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok("Not implemented");
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
                if (rent.RentDays <= 0)
                {
                    return BadRequest("Invalid number of rental days.");
                }

                int result = rent.Rent();
                if (result > 0)
                    return Ok("🎬 Rental completed successfully.");
                else
                    return BadRequest("Failed to rent the movie.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }





        // PUT api/Movies/{id}
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            return Ok("Not implemented");
        }

        // DELETE api/Movies/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteById(int id)
        {
            try
            {
                bool success = Movies.DeleteMovie(id);
                if (success)
                    return Ok("Movie deleted successfully.");
                else
                    return NotFound("Movie not found or already deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }
    }
}
