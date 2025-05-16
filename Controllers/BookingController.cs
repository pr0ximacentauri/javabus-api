using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using javabus_api.Models;
using System;
using javabus_api.Contexts;

namespace javabus_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public BookingsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByUser(int userId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            if (bookings == null || bookings.Count == 0)
                return NotFound("Tidak ada booking ditemukan.");

            return bookings;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound();

            return booking;
        }

        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking([FromBody] BookingDto dto)
        {
            var booking = new Booking
            {
                Status = "pending",
                UserId = dto.UserId,
                ScheduleId = dto.ScheduleId
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound($"Booking dengan ID {id} tidak ditemukan.");
            }

            booking.Status = dto.Status;
            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Bookings.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); 
        }

    }

    public class BookingDto
    {
        public int UserId { get; set; }
        public int ScheduleId { get; set; }
    }

    public class UpdateStatusDto
    {
        public string Status { get; set; }
    }
}
