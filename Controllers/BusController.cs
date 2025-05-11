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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bus>>> GetBuses()
        {
            var buses = await _context.Buses
                .ToListAsync();

            if (buses == null || buses.Count == 0)
                return NotFound(new { message = "Tidak ada data bus" });

            return Ok(buses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetBus(int id)
        {
            var bus = await _context.Buses
                .FirstOrDefaultAsync(c => c.Id == id);

            if (bus == null)
                return NotFound(new { message = "Bus tidak ditemukan" });

            return Ok(bus);
        }

        [HttpPost, Authorize(Roles = "admin")]
        public async Task<ActionResult<Bus>> CreateBus(Bus bus)
        {
            try
            {
                _context.Buses.Add(bus);
                await _context.SaveChangesAsync();
                CreatedAtAction(nameof(GetBus), new { id = bus.Id }, bus);
                return Ok(new {message = "Berhasil menambahkan data bus"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Gagal menambahkan data bus!", error = ex.Message });
            }
        }

        [HttpPut("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateBus(int id, Bus bus)
        {
            var busData = await _context.Buses.FindAsync(id);
            if (busData == null)
                return NotFound(new { message = "Kota dengan id tersebut tidak ditemukan" });

            busData.Name = bus.Name;
            busData.BusClass = bus.BusClass;
            busData.TotalSeat = bus.TotalSeat;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Berhasil memperbarui data bus" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Gagal memperbarui data bus", error = ex.Message });
            }
        }

        [HttpDelete("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBus(int id)
        {
            var bus = await _context.Buses.FindAsync(id);
            if (bus == null)
                return NotFound(new { message = "Bus tidak ditemukan" });

            _context.Buses.Remove(bus);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bus berhasil di hapus" });
        }
    }
}
