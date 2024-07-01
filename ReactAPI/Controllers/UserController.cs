using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactAPI.Custom;
using ReactAPI.Models;
using ReactAPI.Models.DTOs;

namespace ReactAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly utilities _utilities;
        private readonly DbcakesContext _context;
        public UserController(DbcakesContext context, utilities utilities)
        {
            _context = context;
            _utilities = utilities;
        }

        [HttpGet]
        [Route("UserList")]
        public async Task<IActionResult> UserList()
        {
            var users = await _context.Users.ToListAsync();

            if (users == null)
            {
                return NotFound();
            }

            return StatusCode(StatusCodes.Status200OK, new { value = users });
        }

        [HttpGet]
        [Route("GetUser")]
        public async Task<ActionResult<UserDTO>> GetUser(int user_id)
        {
            var user = await _context.Users.FindAsync(user_id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(new UserDTO
            {
                Name = user.Name,
                Email = user.Email,
                
            });
        }

        [HttpPut]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser(int user_id, [FromBody] UserDTO userDto)
        {
            var userFound = await _context.Users.FindAsync(user_id);
            if (userFound == null)
            {
                return NotFound(new { isSuccess = false });
            }

            var emailExists = await _context.Users.AnyAsync(u => u.Email == userDto.Email && u.Id != user_id);
            if (emailExists)
            {
                return Conflict(new { isSuccess = false });
            }

            userFound.Name = userDto.Name;
            userFound.Email = userDto.Email;

            _context.Users.Update(userFound);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
        }


        //Eliminar usuario con todo y registros
        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int user_id, [FromBody] LoginDTO deleteUser)
        {
            var user = await _context.Users
                .Include(u => u.Ratings)
                .FirstOrDefaultAsync(u => u.Id == user_id);

            if (user == null)
            {
                return NotFound(new { isSuccess = false });
            }

            if (user.Password != _utilities.encryptSHA256(deleteUser.Password))
            {
                return Unauthorized(new { isSuccess = false });
            }

            if (user.Ratings != null && user.Ratings.Any())
            {
                _context.Ratings.RemoveRange(user.Ratings);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
        }


    }
}
