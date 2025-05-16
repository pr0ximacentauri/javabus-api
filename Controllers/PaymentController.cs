using javabus_api.Models;
using Microsoft.AspNetCore.Mvc;
using javabus_api.Contexts;

namespace javabus_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly MidtransService _midtransService;

        public PaymentController(MidtransService midtransService)
        {
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
    }

    public class PaymentRequestDto
    {
        public int BookingId { get; set; }
        public int GrossAmount { get; set; }
    }
}
