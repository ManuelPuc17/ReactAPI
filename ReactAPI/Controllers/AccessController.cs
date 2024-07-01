using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactAPI.Custom;
using ReactAPI.Models;
using ReactAPI.Models.DTOs;

namespace ReactAPI.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly DbcakesContext _context;
        private readonly utilities _utilities;
        public AccessController(DbcakesContext context, utilities utilities)
        {
            _context = context;
            _utilities = utilities;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(UserDTO user)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (emailExists)
            {
                return Conflict(new { isSuccess = false });
            }

            var model = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = _utilities.encryptSHA256(user.Password)
            };

            await _context.Users.AddAsync(model);
            await _context.SaveChangesAsync();

            if (model.Id != 0)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = false });
            }
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult>Login(LoginDTO login)
        {
            var userFound = await _context.Users.Where(u => u.Email == login.Email &&
            u.Password == _utilities.encryptSHA256(login.Password)).FirstOrDefaultAsync();

            if(userFound == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { isSuccess = false, token = "", userId = "", userName = "" });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, new
                       {
                            isSuccess = true,
                            token = _utilities.generateJwt(userFound),
                            userId = userFound.Id.ToString(),
                            userName = userFound.Name
                       });
            }
        }


    }
}
