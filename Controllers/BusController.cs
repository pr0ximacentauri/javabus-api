using javabus_api.Contexts;
using javabus_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;


namespace javabus_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public BusController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("api/javabus")]
        public async Task<ActionResult<IEnumerable<Bus>>> GetBuses()
        {
            var buses = await _context.Buses
                .ToListAsync();

            if (buses == null || buses.Count == 0)
                return NotFound(new { message = "No buses found" });

            return Ok(buses);
        }

        [HttpGet("api/javabus/{id}")]
        public async Task<ActionResult<City>> GetBus(int id)
        {
            var bus = await _context.Buses
                .FirstOrDefaultAsync(c => c.Id == id);

            if (bus == null)
                return NotFound(new { message = "Bus not found" });

            return Ok(bus);
        }

        [HttpPost("api/javabus"), Authorize(Roles = "admin")]
        public async Task<ActionResult<Bus>> CreateBus(Bus bus)
        {
            try
            {
                _context.Buses.Add(bus);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetBus), new { id = bus.Id }, bus);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error creating bus", error = ex.Message });
            }
        }

        [HttpPut("api/javabus/{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateBus(int id, Bus bus)
        {
            if (id != bus.Id)
                return BadRequest(new { message = "ID mismatch" });

            _context.Entry(bus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(bus);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Cities.Any(e => e.Id == id))
                    return NotFound(new { message = "Bus not found" });

                throw;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error updating bus", error = ex.Message });
            }
        }

        [HttpDelete("api/javabus/{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBus(int id)
        {
            var bus = await _context.Buses.FindAsync(id);
            if (bus == null)
                return NotFound(new { message = "Bus not found" });

            _context.Buses.Remove(bus);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bus deleted" });
        }
    }
}
