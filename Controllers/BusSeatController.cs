using javabus_api.Contexts;
using javabus_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace javabus_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusSeatController : ControllerBase
    {
        private readonly Contexts.ApplicationDBContext _context;
        public BusSeatController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("bus/{busId}")]
        public async Task<ActionResult<IEnumerable<BusSeat>>> GetSeatsByBus(int busId)
        {
            var seats = await _context.BusSeats
                .Where(bs => bs.BusId == busId)
                .ToListAsync();

            return Ok(seats);
        }
    }
}
