using javabus_api.Contexts;
using javabus_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace javabus_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvinceController : ControllerBase
    {
        public readonly ApplicationDBContext _context;
        public ProvinceController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Province>>> GetProvinces()
        {
            var cities = await _context.Provinces
                .ToListAsync();

            if (cities == null || cities.Count == 0)
                return NotFound(new { message = "Tidak ada data provinsi" });

            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Province>> GetProvince(int id)
        {
            var province = await _context.Provinces
                .FirstOrDefaultAsync(c => c.Id == id);

            if (province == null)
                return NotFound(new { message = "Provinsi tidak ditemukan" });

            return Ok(province);
        }

        [HttpPost, Authorize(Roles = "admin")]
        public async Task<ActionResult<Province>> CreateProvince(Province province)
        {
            try
            {
                _context.Provinces.Add(province);
                await _context.SaveChangesAsync();
                CreatedAtAction(nameof(GetProvince), new { id = province.Id }, province);
                return Ok(new { message = "Berhasil menambahkan data provinsi" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Gagal menambahkan data provinsi!", error = ex.Message });
            }
        }

        [HttpPut("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCity(int id, Province province)
        {
            var cityData = await _context.Provinces.FindAsync(id);
            if (cityData == null)
                return NotFound(new { message = "Provinsi dengan id tersebut tidak ditemukan" });

            cityData.Name = province.Name;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { messsage = "Berhasil memperbarui data provinsi" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Gagal memperbarui data provinsi!", error = ex.Message });
            }
        }

        [HttpDelete("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var province = await _context.Provinces.FindAsync(id);
            if (province == null)
                return NotFound(new { message = "Provinsi tidak ditemukan" });

            _context.Provinces.Remove(province);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Provinsi berhasil dihapus" });
        }
    }
}
