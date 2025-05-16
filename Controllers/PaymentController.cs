using javabus_api.Models;
using javabus_api.Services;
using Microsoft.AspNetCore.Mvc;
using javabus_api.Contexts;
using System.Text.Json;

namespace javabus_api.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly MidtransService _midtransService;
        private readonly ApplicationDBContext _context;

        public PaymentController(MidtransService midtransService, ApplicationDBContext context)
        {
            _midtransService = midtransService;
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDto dto)
        {
            try
            {
                var booking = new Booking
                {
                    Status = "pending",
                    UserId = dto.UserId,
                    ScheduleId = dto.ScheduleId
                };
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                var response = await _midtransService.CreateTransactionAsync(booking.Id, dto.GrossAmount);
                var json = JsonDocument.Parse(response);

                var orderId = json.RootElement.GetProperty("order_id").GetString();
                var paymentType = json.RootElement.GetProperty("payment_type").GetString();
                var paymentUrl = json.RootElement.TryGetProperty("redirect_url", out var redirectUrl)
                    ? redirectUrl.GetString()
                    : json.RootElement.TryGetProperty("qr_string", out var qrString)
                        ? qrString.GetString()
                        : null;


                var payment = new Payment
                {
                    BookingId = booking.Id,
                    OrderId = orderId, 
                    GrossAmount = dto.GrossAmount,
                    PaymentType = paymentType,
                    PaymentUrl = paymentUrl,
                    TransactionStatus = "pending",
                    TransactionTime = DateTime.UtcNow
                };
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    payment_url = payment.PaymentUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = $"Gagal membuat transaksi: {ex.Message}"
                });
            }
        }
    }

    public class PaymentRequestDto
    {
        public int UserId { get; set; }
        public int ScheduleId { get; set; }
        public int GrossAmount { get; set; }
    }
}
