using javabus_api.Contexts;
using javabus_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace javabus_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatBookingController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public SeatBookingController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SeatBooking>>> GetSeatBookings()
        {
            var bookings = await _context.SeatBookings
                .Include(sb => sb.Booking)
                .Include(sb => sb.Schedule)
                .Include(sb => sb.Seat)
                .ToListAsync();

            if (bookings == null || bookings.Count == 0)
                return NotFound(new { message = "Tidak ada data kursi yang dipesan" });

            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SeatBooking>> GetSeatBooking(int id)
        {
            var seatBooking = await _context.SeatBookings
                .Include(sb => sb.Booking)
                .Include(sb => sb.Schedule)
                .Include(sb => sb.Seat)
                .FirstOrDefaultAsync(sb => sb.Id == id);

            if (seatBooking == null)
                return NotFound(new { message = "Tidak ditemukan kursi yang dipesan" });

            return Ok(seatBooking);
        }

        [HttpGet("schedule/{scheduleId}")]
        public async Task<ActionResult<IEnumerable<SeatBooking>>> GetSeatBookingBySchedule(int scheduleId)
        {
            var bookings = _context.SeatBookings
                .Where(sb => sb.ScheduleId == scheduleId)
                .ToListAsync();

            return Ok(bookings);
        }

        [HttpPost]
        public async Task<ActionResult<SeatBooking>> CreateSeatBooking(SeatBooking seatBooking)
        {
            var exists = await _context.SeatBookings
                .AnyAsync(sb => sb.ScheduleId == seatBooking.ScheduleId && sb.SeatId == seatBooking.SeatId);

            if (exists)
                return Conflict(new { message = "Kursi ini sudah dipesan untuk jadwal tersebut" });

            try
            {
                _context.SeatBookings.Add(seatBooking);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetSeatBooking), new { id = seatBooking.Id }, seatBooking);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Gagal menambahkan kursi yang dipesan", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSeatBooking(int id, SeatBooking updated)
        {
            var seatBooking = await _context.SeatBookings.FindAsync(id);
            if (seatBooking == null)
                return NotFound(new { message = "Tidak ditemukan kursi yang dipesan" });

            if (seatBooking.ScheduleId != updated.ScheduleId || seatBooking.SeatId != updated.SeatId)
            {
                var duplicate = await _context.SeatBookings
                    .AnyAsync(sb => sb.Id != id && sb.ScheduleId == updated.ScheduleId && sb.SeatId == updated.SeatId);
                if (duplicate)
                    return Conflict(new { message = "Kursi ini sudah dipesan untuk jadwal tersebut" });
            }

            seatBooking.BookingId = updated.BookingId;
            seatBooking.ScheduleId = updated.ScheduleId;
            seatBooking.SeatId = updated.SeatId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Berhasil memperbarui kursi yang dipesan" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Gagal memperbarui kursi yang dipesan", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeatBooking(int id)
        {
            var seatBooking = await _context.SeatBookings.FindAsync(id);
            if (seatBooking == null)
                return NotFound(new { message = "Tidak ditemukan kursi yang dipesan" });

            _context.SeatBookings.Remove(seatBooking);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kursi yang dipesan berhasil dihapus" });
        }
    }
}
