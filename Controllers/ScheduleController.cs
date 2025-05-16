using javabus_api.Contexts;
using javabus_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace javabus_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public ScheduleController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetSchedules()
        {
            var schedules = await _context.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .ToListAsync();

            if (schedules == null || schedules.Count == 0)
                return NotFound(new { message = "Tidak ada data jadwal" });

            return Ok(schedules);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Schedule>>> SearchSchedules([FromQuery] int routeId, [FromQuery] string date)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
            {
                return BadRequest(new { message = "Format tanggal tidak valid" });
            }

            var startOfDayUtc = DateTime.SpecifyKind(parsedDate.Date, DateTimeKind.Utc);
            var endOfDayUtc = DateTime.SpecifyKind(parsedDate.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

            var schedules = await _context.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .Where(s => s.RouteId == routeId &&
                            s.DepartureTime >= startOfDayUtc &&
                            s.DepartureTime <= endOfDayUtc)
                .ToListAsync();

            if (schedules == null || schedules.Count == 0)
                return NotFound(new { message = "Jadwal tidak ditemukan" });

            return Ok(schedules);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Schedule>> GetSchedule(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
                return NotFound(new { message = "Jadwal tidak ditemukan" });

            return Ok(schedule);
        }

        [HttpPost, Authorize(Roles = "admin")]
        public async Task<ActionResult> CreateSchedule(Schedule schedule)
        {
            if (schedule.DepartureTime == default ||
                schedule.TicketPrice <= 0 ||
                schedule.BusId <= 0 ||
                schedule.RouteId <= 0)
            {
                return BadRequest(new { message = "Semua field wajib diisi dengan benar." });
            }

            bool exists = await _context.Schedules
                .AnyAsync(s => s.DepartureTime == schedule.DepartureTime && s.BusId == schedule.BusId);

            //if (exists)
            //{
            //    return Conflict(new { message = "Jadwal dengan bus dan waktu keberangkatan tersebut sudah ada." });
            //}

            try
            {
                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetSchedule), new { id = schedule.Id }, new { message = "Berhasil menambahkan jadwal", schedule });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Gagal menambahkan jadwal!", error = ex.Message });
            }
        }

        [HttpPut("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateSchedule(int id, Schedule schedule)
        {
            var existing = await _context.Schedules.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Jadwal tidak ditemukan" });

            if (schedule.DepartureTime == default ||
                schedule.TicketPrice <= 0 ||
                schedule.BusId <= 0 ||
                schedule.RouteId <= 0)
            {
                return BadRequest(new { message = "Semua field wajib diisi dengan benar." });
            }

            bool duplicate = await _context.Schedules
                .AnyAsync(s => s.Id != id &&
                               s.DepartureTime == schedule.DepartureTime &&
                               s.BusId == schedule.BusId);

            if (duplicate)
            {
                return Conflict(new { message = "Jadwal dengan bus dan waktu keberangkatan tersebut sudah ada." });
            }

            existing.DepartureTime = schedule.DepartureTime;
            existing.TicketPrice = schedule.TicketPrice;
            existing.BusId = schedule.BusId;
            existing.RouteId = schedule.RouteId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Berhasil memperbarui jadwal" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Gagal memperbarui jadwal!", error = ex.Message });
            }
        }

        [HttpDelete("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
                return NotFound(new { message = "Jadwal tidak ditemukan" });

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Jadwal berhasil dihapus" });
        }
    }
}
