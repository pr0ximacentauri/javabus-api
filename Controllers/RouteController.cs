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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Route>>> GetRoutes()
        {
            var routes = await _context.Routes
                .Include(r => r.OriginCity)
                .Include(r => r.DestinationCity)
                .ToListAsync();
            if (routes == null || routes.Count == 0)
                return NotFound(new { message = "Data rute perjalanan tidak ditemukan" });

            return Ok(routes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Route>> GetRoute(int id)
        {
            var route = await _context.Routes
                .Include(r => r.OriginCity)
                .Include(r => r.DestinationCity)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
                return NotFound(new { message = "Data rute perjalanan tidak ditemukan" });

            return route;
        }

        [HttpPost, Authorize(Roles = "admin")]
        public async Task<ActionResult<Models.Route>> CreateRoute(Models.Route route)
        {
            try
            {
                _context.Routes.Add(route);
                await _context.SaveChangesAsync();
                CreatedAtAction(nameof(GetRoute), new { id = route.Id }, route);
                return Ok(new { message = "Berhasil menambahkan rute perjalanan" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Gagal menambahkan rute perjalanan", error = ex.Message });
            }
        }

        [HttpPut("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateRoute(int id, Models.Route route)
        {
            var routeData = await _context.Routes.FindAsync(id);
            if (routeData == null)
                return NotFound(new { message = "Rute perjalanan dengan id tersebut tidak ditemukan" });

            routeData.OriginCityId = route.OriginCityId;
            routeData.DestinationCityId = route.DestinationCityId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(route);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Gagal update data rute perjalanan", error = ex.Message });
            }
        }

        [HttpDelete("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route == null)
                return NotFound(new { message = "Data rute perjalanan tidak ditemukan" });

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Rute perjalanan berhasil dihapus" });
        }
    }
}
