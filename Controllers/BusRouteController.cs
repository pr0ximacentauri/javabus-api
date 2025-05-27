using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using javabus_api.Models;
using Microsoft.AspNetCore.Authorization;

namespace javabus_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusRouteController : ControllerBase
    {
        private readonly Contexts.ApplicationDBContext _context;

        public BusRouteController(Contexts.ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusRoute>>> GetRoutes()
        {
            var routes = await _context.Routes
                .Include(r => r.OriginCity)
                .Include(r => r.DestinationCity)
                .ToListAsync();
            if (routes == null || routes.Count == 0)
                return NotFound(new { message = "Data rute perjalanan tidak ditemukan" });

            return Ok(routes);
        }

        [HttpGet("origins")]
        public async Task<ActionResult<IEnumerable<City>>> GetAvailableOrigins()
        {
            var origins = await _context.Routes
                .Include(r => r.OriginCity)
                .Select(r => r.OriginCity)
                .Distinct()
                .ToListAsync();

            if (origins.Count == 0)
                return NotFound(new { message = "Tidak ada kota asal tersedia" });

            return Ok(origins);
        }

        [HttpGet("destinations/{originId}")]
        public async Task<ActionResult<IEnumerable<City>>> GetAvailableDestinations(int originId)
        {
            var destinations = await _context.Routes
                .Where(r => r.OriginCityId == originId)
                .Include(r => r.DestinationCity)
                .Select(r => r.DestinationCity)
                .Distinct()
                .ToListAsync();

            if (destinations.Count == 0)
                return NotFound(new { message = "Tidak ada tujuan dari kota asal ini" });

            return Ok(destinations);
        }

        //[HttpGet("check")]
        //public async Task<IActionResult> CheckRoute([FromQuery] int originId, [FromQuery] int destinationId)
        //{
        //    var exists = await _context.Routes.AnyAsync(r =>
        //        r.OriginCityId == originId && r.DestinationCityId == destinationId);

        //    if (!exists)
        //        return NotFound(new { message = "Terminal asal Anda tidak memiliki route ke terminal tujuan ini" });

        //    return Ok(new { message = "Rute tersedia antara kota asal dan tujuan" });
        //

        [HttpGet("{id}")]
        public async Task<ActionResult<BusRoute>> GetRoute(int id)
        {
            var route = await _context.Routes
                .Include(r => r.OriginCity)
                .Include(r => r.DestinationCity)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
                return NotFound(new { message = "Data rute perjalanan tidak ditemukan" });

            return route;
        }

        [HttpGet("by-city")]
        public async Task<ActionResult> GetRouteId([FromQuery] int originId, [FromQuery] int destinationId)
        {
            var route = await _context.Routes
                .FirstOrDefaultAsync(r => r.OriginCityId == originId && r.DestinationCityId == destinationId);

            if (route == null)
                return NotFound(new { message = "Rute tidak ditemukan" });

            return Ok(new { routeId = route.Id });
        }


        [HttpPost, Authorize(Roles = "admin")]
        public async Task<ActionResult<BusRoute>> CreateRoute(BusRoute route)
        {
            if (route.OriginCityId == 0 || route.DestinationCityId == 0)
                return BadRequest(new { message = "Kota asal dan tujuan wajib diisi" });

            if (route.OriginCityId == route.DestinationCityId)
                return BadRequest(new { message = "Kota asal dan tujuan tidak boleh sama" });

            var originExists = await _context.Cities.AnyAsync(c => c.Id == route.OriginCityId);
            var destinationExists = await _context.Cities.AnyAsync(c => c.Id == route.DestinationCityId);

            if (!originExists || !destinationExists)
                return BadRequest(new { message = "Kota asal atau tujuan tidak ditemukan di database" });

            var isDuplicate = await _context.Routes
                .AnyAsync(r => r.OriginCityId == route.OriginCityId && r.DestinationCityId == route.DestinationCityId);

            if (isDuplicate)
                return Conflict(new { message = "Rute dengan kota asal dan tujuan tersebut sudah ada" });

            try
            {
                _context.Routes.Add(route);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetRoute), new { id = route.Id }, route);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Gagal menambahkan rute perjalanan", error = ex.Message });
            }
        }

        [HttpPut("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateRoute(int id, Models.BusRoute route)
        {
            if (route.OriginCityId == 0 || route.DestinationCityId == 0)
                return BadRequest(new { message = "Kota asal dan tujuan wajib diisi" });

            if (route.OriginCityId == route.DestinationCityId)
                return BadRequest(new { message = "Kota asal dan tujuan tidak boleh sama" });

            var originExists = await _context.Cities.AnyAsync(c => c.Id == route.OriginCityId);
            var destinationExists = await _context.Cities.AnyAsync(c => c.Id == route.DestinationCityId);

            if (!originExists || !destinationExists)
                return BadRequest(new { message = "Kota asal atau tujuan tidak ditemukan di database" });

            var routeData = await _context.Routes.FindAsync(id);
            if (routeData == null)
                return NotFound(new { message = "Rute perjalanan dengan id tersebut tidak ditemukan" });

            var isDuplicate = await _context.Routes
                .AnyAsync(r => r.Id != id &&
                               r.OriginCityId == route.OriginCityId &&
                               r.DestinationCityId == route.DestinationCityId);

            if (isDuplicate)
                return Conflict(new { message = "Rute dengan kota asal dan tujuan tersebut sudah ada" });

            routeData.OriginCityId = route.OriginCityId;
            routeData.DestinationCityId = route.DestinationCityId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Berhasil memperbarui rute perjalanan" });
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
