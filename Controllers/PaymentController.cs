using javabus_api.Models;
using Microsoft.AspNetCore.Mvc;
using javabus_api.Contexts;
using Microsoft.EntityFrameworkCore;

namespace javabus_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly MidtransService _midtransService;

        public PaymentController(MidtransService midtransService, ApplicationDBContext context)
        {
            _context = context;
            _midtransService = midtransService;
        }

        [HttpPost("snap")]
        public async Task<IActionResult> CreateSnapPayment([FromBody] PaymentRequestDto dto)
        {
            try
            {
                var redirectUrl = await _midtransService.CreateSnapTransactionAsync(dto.BookingId, dto.GrossAmount);

                return Ok(new
                {
                    payment_url = redirectUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = $"Failed to create payment: {ex.Message}"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _context.Payments.ToListAsync();
            return Ok(payments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound(new { message = "Pembayaran tidak ditemukan." });

            return Ok(payment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] Payment updated)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound(new { message = "Pembayaran tidak ditemukan." });

            payment.TransactionStatus = updated.TransactionStatus;
            payment.PaymentType = updated.PaymentType;
            payment.PaymentUrl = updated.PaymentUrl;
            payment.TransactionTime = updated.TransactionTime;
            payment.GrossAmount = updated.GrossAmount;
            payment.OrderId = updated.OrderId;

            await _context.SaveChangesAsync();
            return Ok(payment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound(new { message = "Pembayaran tidak ditemukan." });

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pembayaran berhasil dihapus." });
        }
    }

    public class PaymentRequestDto
    {
        public int BookingId { get; set; }
        public int GrossAmount { get; set; }
    }
}
