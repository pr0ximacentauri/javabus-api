using javabus_api.Contexts;
using javabus_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace javabus_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public CityController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("api/javabus")]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            var cities = await _context.Cities
                .Include(c => c.Province)
                .ToListAsync();

            if (cities == null || cities.Count == 0)
                return NotFound(new { message = "No cities found" });

            return Ok(cities);
        }

        [HttpGet("api/javabus/{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            var city = await _context.Cities
                .Include(c => c.Province)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
                return NotFound(new { message = "City not found" });

            return Ok(city);
        }

        [HttpPost("api/javabus"), Authorize(Roles = "admin")]
        public async Task<ActionResult<City>> CreateCity(City city)
        {
            try
            {
                _context.Cities.Add(city);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCity), new { id = city.Id }, city);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error creating city", error = ex.Message });
            }
        }

        [HttpPut("api/javabus/{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCity(int id, City city)
        {
            if (id != city.Id)
                return BadRequest(new { message = "ID mismatch" });

            _context.Entry(city).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(city);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Cities.Any(e => e.Id == id))
                    return NotFound(new { message = "City not found" });

                throw;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error updating city", error = ex.Message });
            }
        }

        [HttpDelete("api/javabus/{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
                return NotFound(new { message = "City not found" });

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return Ok(new { message = "City deleted" });
        }
    }
}
