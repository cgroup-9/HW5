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
                bool success = movie.Insert();
                if (success)
                    return Ok("Movie inserted successfully.");
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
            try
            {
                int insertedCount = 0;
                foreach (var movie in movies)
                {
                    // לדוגמה - הכנסת כל סרט למסד נתונים
                    if (movie.Insert())
                        insertedCount++;
                }

                return Ok(new
                {
                    Inserted = insertedCount,
                    Total = movies.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ שגיאה בשרת: {ex.Message}");
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
