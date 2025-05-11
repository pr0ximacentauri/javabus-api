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
            if (string.IsNullOrWhiteSpace(bus.Name) || string.IsNullOrWhiteSpace(bus.BusClass) || bus.TotalSeat <= 0)
            {
                return BadRequest(new { message = "Semua field harus diisi dengan benar!" });
            }

            bool busExists = await _context.Buses
                .AnyAsync(b => b.Name.ToLower() == bus.Name.ToLower() && b.BusClass.ToLower() == bus.BusClass.ToLower());

            if (busExists)
            {
                return Conflict(new { message = "Bus dengan nama dan kelas yang sama sudah ada" });
            }

            try
            {
                _context.Buses.Add(bus);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetBus), new { id = bus.Id }, new { message = "Berhasil menambahkan data bus", bus });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Gagal menambahkan data bus!", error = ex.Message });
            }
        }

        [HttpPut("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateBus(int id, Bus bus)
        {
            if (string.IsNullOrWhiteSpace(bus.Name) || string.IsNullOrWhiteSpace(bus.BusClass) || bus.TotalSeat <= 0)
            {
                return BadRequest(new { message = "Semua field harus diisi dengan benar." });
            }

            var busData = await _context.Buses.FindAsync(id);
            if (busData == null)
                return NotFound(new { message = "Bus dengan id tersebut tidak ditemukan" });

            bool duplicate = await _context.Buses
                .AnyAsync(b => b.Id != id && b.Name.ToLower() == bus.Name.ToLower() && b.BusClass.ToLower() == bus.BusClass.ToLower());

            if (duplicate)
            {
                return Conflict(new { message = "Bus dengan nama dan kelas yang sama sudah ada" });
            }

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
