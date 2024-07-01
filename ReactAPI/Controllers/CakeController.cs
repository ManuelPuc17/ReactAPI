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
    public class CakeController : ControllerBase
    {
        private readonly DbcakesContext _context;
        public CakeController(DbcakesContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("CakeList")]
        public async Task<IActionResult> CakeList()
        {
            var cake = await _context.Cakes
                .Include(c => c.Ratings)
                .Select(c => new CakeDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Origin = c.Origin,
                    Price = c.Price ?? 0,
                    ImageUrl = c.ImageUrl,
                    AverageFlavor = c.Ratings.Any() ? Convert.ToDouble(c.Ratings.Average(r => r.Flavor)) : 0,
                    AveragePresentation = c.Ratings.Any() ? Convert.ToDouble(c.Ratings.Average(r => r.Presentation)) : 0,
                    Finalaverage = c.Ratings.Any() ? (Convert.ToDouble(c.Ratings.Average(r => r.Flavor)) + Convert.ToDouble(c.Ratings.Average(r => r.Presentation))) / 2 : 0
                })
                .ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = cake });

        }

        [HttpGet]
        [Route("Getacake")]
        public async Task<ActionResult<CakeDTO>> GetCake(int cake_id)
        {
            var cake = await _context.Cakes.FindAsync(cake_id);
            if(cake == null)
            {
                return NotFound();
            }
            return Ok(new CakeDTO
            {
                Id = cake.Id,
                Name = cake.Name,
                Origin = cake.Origin,
                Price = cake.Price ?? 0,
                ImageUrl = cake.ImageUrl
            });
        }

        [HttpGet]
        [Route("CakeNotRating")]
        public async Task<ActionResult<CakeDTO>> GetCakesNotRating(int user_id)
        {
            var RatingIds = await _context.Ratings
                .Where(r => r.UserId == user_id)
                .Select(r => r.CakeId)
                .ToListAsync();

            var CakesNotRatings = await _context.Cakes
                .Where(c => !RatingIds.Contains(c.Id))
                .Select(c => new CakeDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = c.ImageUrl
                })
                .ToListAsync();

            return Ok(CakesNotRatings);
        }

        [HttpPost]
        [Route("AddCake")]
        public async Task<IActionResult> AddCake(CakeDTO modelcake)
        {
            var cake = new Cake
            {
                Name = modelcake.Name,
                Origin = modelcake.Origin,
                Price = modelcake.Price,
                ImageUrl = modelcake.ImageUrl
            };
            await _context.Cakes.AddAsync(cake);
            await _context.SaveChangesAsync();

            if(cake.Id != 0)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            }
            else
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, new { isSuccess = false });
            }
        }

        [HttpPut]
        [Route("UpdateCake")]
        public async Task<IActionResult> UpdateCake(int cake_id, [FromBody] CakeDTO cake)
        {
            var cakeFound = await _context.Cakes.FindAsync(cake_id);
            if(cakeFound == null)
            {
                return NotFound(new { isSuccess = false });
            }

            cakeFound.Name = cake.Name;
            cakeFound.Origin = cake.Origin;
            cakeFound.Price = cake.Price;
            cakeFound.ImageUrl = cake.ImageUrl;


            _context.Cakes.Update(cakeFound);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });

        }

        //Eliminar pastel con todo y registros
        [HttpDelete]
        [Route("DeleteCake")]
        public async Task<IActionResult> DeleteCake(int cake_id)
        {
            var cake = await _context.Cakes
                .Include(c => c.Ratings)
                .FirstOrDefaultAsync(c => c.Id == cake_id);

            if (cake == null)
            {
                return NotFound(new { isSuccess = false });
            }

            if (cake.Ratings != null && cake.Ratings.Any())
            {
                _context.Ratings.RemoveRange(cake.Ratings);
            }

            _context.Cakes.Remove(cake);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
        }

    }
}   
