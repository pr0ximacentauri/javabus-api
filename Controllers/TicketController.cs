using javabus_api.Contexts;
using javabus_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace javabus_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public TicketsController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets([FromQuery] int? bookingId)
        {
            try
            {
                var query = _context.Tickets.AsQueryable();
                if (bookingId.HasValue)
                {
                    query = query.Where(t => t.BookingId == bookingId);
                }
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(int id)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(id);
                if (ticket == null)
                    return NotFound();

                return ticket;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Ticket>> CreateTicket(Ticket ticket)
        {
            try
            {
                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create ticket: {ex.Message}");
            }
        }

        [HttpPost("snapshot/{bookingId}")]
        public async Task<IActionResult> CreateTicketSnapshot(int bookingId)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Bus)
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Route)
                    .FirstOrDefaultAsync(b => b.Id == bookingId);

                if (booking == null)
                    return NotFound("Booking tidak ditemukan.");

                var route = await _context.Routes
                    .Include(r => r.OriginCity)
                    .Include(r => r.DestinationCity)
                    .FirstOrDefaultAsync(r => r.Id == booking.Schedule.RouteId);

                if (route == null)
                    return NotFound("Rute tidak ditemukan.");

                var seatBookings = await _context.SeatBookings
                    .Where(sb => sb.BookingId == bookingId)
                    .ToListAsync();

                if (!seatBookings.Any())
                    return BadRequest("Tidak ada kursi yang dibooking.");

                var tickets = new List<Ticket>();
                foreach (var sb in seatBookings)
                {
                    var ticket = new Ticket
                    {
                        BookingId = booking.Id,
                        SeatId = sb.SeatId,
                        DepartureTime = booking.Schedule.DepartureTime,
                        OriginCity = route.OriginCity.Name,
                        DestinationCity = route.DestinationCity.Name,
                        BusName = booking.Schedule.Bus.Name,
                        BusClass = booking.Schedule.Bus.BusClass,
                        TicketPrice = booking.Schedule.TicketPrice,
                        QrCodeUrl = "", 
                        TicketStatus = "belum digunakan"
                    };

                    tickets.Add(ticket);
                }

                _context.Tickets.AddRange(tickets);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Tiket berhasil dibuat", count = tickets.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Gagal membuat tiket", detail = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, Ticket updatedTicket)
        {
            if (id != updatedTicket.Id)
                return BadRequest("ID mismatch");

            try
            {
                _context.Entry(updatedTicket).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tickets.Any(t => t.Id == id))
                    return NotFound();

                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Update error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(id);
                if (ticket == null)
                    return NotFound();

                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Delete error: {ex.Message}");
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateTicketStatus(int id, [FromBody] string newStatus)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(id);
                if (ticket == null)
                    return NotFound();

                ticket.TicketStatus = newStatus;
                await _context.SaveChangesAsync();
                return Ok(ticket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to update status: {ex.Message}");
            }
        }
    }
}
