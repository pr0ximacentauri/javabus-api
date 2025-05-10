using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using javabus_api.Models;
using Microsoft.AspNetCore.Authorization;

namespace javabus_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly Contexts.ApplicationDBContext _context;

        public RouteController(Contexts.ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("api/javabus")]
        public async Task<ActionResult<IEnumerable<Models.Route>>> GetRoutes()
        {
            var routes = await _context.Routes
                .Include(r => r.OriginCity)
                .Include(r => r.DestinationCity)
                .ToListAsync();
            if (routes == null || routes.Count == 0)
                return NotFound(new { message = "No bus routes found" });

            return Ok(routes);
        }

        [HttpGet("api/javabus/{id}")]
        public async Task<ActionResult<Models.Route>> GetRoute(int id)
        {
            var route = await _context.Routes
                .Include(r => r.OriginCity)
                .Include(r => r.DestinationCity)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
                return NotFound(new { message = "Route not found" });

            return route;
        }

        [HttpPost("api/javabus"), Authorize(Roles = "admin")]
        public async Task<ActionResult<Models.Route>> CreateRoute(Models.Route route)
        {
            try
            {
                _context.Routes.Add(route);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetRoute), new { id = route.Id }, route);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error creating route", error = ex.Message });
            }
        }

        [HttpPut("api/javabus/{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateRoute(int id, Models.Route route)
        {
            if (id != route.Id)
                return BadRequest(new { message = "ID mismatch" });

            _context.Entry(route).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(route);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Routes.Any(e => e.Id == id))
                    return NotFound(new { message = "Route not found" });

                throw;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Update failed", error = ex.Message });
            }
        }

        [HttpDelete("api/javabus/{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route == null)
                return NotFound(new { message = "Route not found" });

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Route deleted" });
        }
    }
}
