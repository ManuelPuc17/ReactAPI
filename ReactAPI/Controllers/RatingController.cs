using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactAPI.Models;
using ReactAPI.Models.DTOs;

namespace ReactAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly DbcakesContext _context;
        public RatingController(DbcakesContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("RatingList")]
        public async Task<IActionResult> RatingList()
        {
            var rating = await _context.Ratings
                .Include(r => r.User)
                .Include(r => r.Cake)
                .Select(r => new RatingDTO
                {
                    Id = r.Id,
                    UserId = r.User.Id,
                    UserName = r.User.Name,
                    CakeId = r.Cake.Id,
                    CakeName = r.Cake.Name,
                    Flavor = r.Flavor,
                    Presentation = r.Presentation,
                    CakeImageUrl = r.Cake.ImageUrl
    })
                .ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = rating });
        }

        [HttpGet]
        [Route("GetaRating")]
        public async Task<IActionResult> GetRating(int rating_id)
        {
            var rating = await _context.Ratings
                .Include(r => r.User)
                .Include(r => r.Cake)
                .Where(r => r.Id == rating_id)
                .Select(r => new RatingDTO
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserName = r.User.Name,
                    CakeId = r.CakeId,
                    CakeName = r.Cake.Name,
                    Flavor = r.Flavor,
                    Presentation = r.Presentation,
                    CakeImageUrl = r.Cake.ImageUrl
                })
                .FirstOrDefaultAsync();
            if(rating == null)
            {
                return NotFound();
            }
                return Ok(rating);
        }

        [HttpGet]
        [Route("GetMyrating")]
        public async Task<IActionResult> MyRating(int user_id)
        {
            var Mylist = await _context.Ratings
                .Include(r => r.User)
                .Include(r => r.Cake)
                .Where(r => r.User.Id == user_id)
                .Select(r => new RatingDTO
                {
                    Id = r.Id,
                    UserName = r.User.Name,
                    CakeName = r.Cake.Name,
                    Flavor = r.Flavor,
                    Presentation = r.Presentation,
                    CakeImageUrl = r.Cake.ImageUrl
                })
                .ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = Mylist });
        }

        [HttpPost]
        [Route("AddRating")]
        public async Task<IActionResult> AddRating(RatingDTO rating)
        {
            var RatingFound = await _context.Ratings.FirstOrDefaultAsync(r => 
            r.UserId == rating.UserId && r.CakeId == rating.CakeId);

            if(RatingFound != null)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { isSuccess = false });
            }
            var NewRating = new Rating
            {
                UserId = rating.UserId,
                CakeId = rating.CakeId,
                Flavor = rating.Flavor,
                Presentation = rating.Presentation
            };
            await _context.Ratings.AddAsync(NewRating);
            await _context.SaveChangesAsync();
             if(NewRating.Id != 0)
             {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
             }
            return StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = false });
        }

        [HttpPut]
        [Route("UpdateRating")]
        public async Task<IActionResult> UpdateRating(int rating_id, [FromBody] RatingDTO rating)
        {
            var RatingFound = await _context.Ratings.FindAsync(rating_id);
            if(RatingFound == null)
            {
                return NotFound(new { isSuccess = false });
            }
            RatingFound.Flavor = rating.Flavor;
            RatingFound.Presentation = rating.Presentation;

            _context.Ratings.Update(RatingFound);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });

        }

        [HttpDelete]
        [Route("DeleteRating")]
        public async Task<IActionResult> DeleteRating(int rating_id)
        {

            var rating = await _context.Ratings.FindAsync(rating_id);
            if (rating == null)
            {
                return NotFound(new { isSuccess = false });
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
        }

    }
}
