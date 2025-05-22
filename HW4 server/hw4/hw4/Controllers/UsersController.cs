using hw4.Project;
using Microsoft.AspNetCore.Mvc;

namespace hw4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // POST: api/User/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] Users user)
        {
            int result = user.Register();

            if (result == 0)
            {
                return Ok(true);
            }
            else if (result == 3)
            {
                return BadRequest("Username or email already exists.");
            }
            else
            {
                return BadRequest("Unknown error occurred.");
            }
        }


        // POST: api/User/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] Users loginUser)
        {
            Users user = new Users();
            Users? existingUser = user.Login(loginUser.Email, loginUser.Password);

            if (existingUser == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(existingUser);
        }
        // GET: api/User
        [HttpGet]
        public ActionResult<IEnumerable<Users>> GetAllUsers()
        {
            return Ok(Users.Read());
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public ActionResult<string> GetUserById(int id)
        {
            return Ok($"You requested user with ID = {id}");
        }

        // PUT: api/User/{id}
        [HttpPut("{id}")]
        public IActionResult PutUserById(int id, [FromBody] string value)
        {
            return Ok($"You updated user {id} with value = {value}");
        }

        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteUserById(int id)
        {
            return Ok($"You deleted user with ID = {id}");
        }


        // PUT: api/User/update-user
        [HttpPut("update-user")]
        public IActionResult UpdateUser([FromBody] Users user)
        {
            int result = user.Update();

            if (result == 0)
            {
                return Ok("User updated successfully.");
            }
            else if (result == 1)
            {
                return NotFound("User not found or deleted.");
            }
            else
            {
                return BadRequest("Unknown error occurred.");
            }
        }

        // DELETE: api/User/delete-by-email/{email}
        [HttpDelete("delete-by-email/{email}")]
        public IActionResult SoftDeleteUserByEmail(string email)
        {
            Users user = new Users();
            int result = user.SoftDeleteByEmail(email);

            if (result == 0)
            {
                return Ok("User deleted successfully.");
            }
            else if (result == 1)
            {
                return NotFound("User not found or already deleted.");
            }
            else
            {
                return BadRequest("Unknown error occurred.");
            }
        }
    }
}
