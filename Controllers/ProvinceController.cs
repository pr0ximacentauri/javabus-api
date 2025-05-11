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
            if (string.IsNullOrWhiteSpace(province.Name))
                return BadRequest(new { message = "Nama provinsi tidak boleh kosong" });

            var duplicateProvince = await _context.Provinces
                .AnyAsync(p => p.Name.ToLower() == province.Name.ToLower());

            if (duplicateProvince)
                return Conflict(new { message = "Provinsi dengan nama tersebut sudah ada" });

            try
            {
                _context.Provinces.Add(province);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetProvince), new { id = province.Id }, province);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Gagal menambahkan data provinsi!", error = ex.Message });
            }
        }

        [HttpPut("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateProvince(int id, Province province)
        {
            if (string.IsNullOrWhiteSpace(province.Name))
                return BadRequest(new { message = "Nama provinsi tidak boleh kosong" });

            var provinceData = await _context.Provinces.FindAsync(id);
            if (provinceData == null)
                return NotFound(new { message = "Provinsi dengan id tersebut tidak ditemukan" });

            var duplicateProvince = await _context.Provinces
                .AnyAsync(p => p.Id != id && p.Name.ToLower() == province.Name.ToLower());

            if (duplicateProvince)
                return Conflict(new { message = "Provinsi dengan nama tersebut sudah ada" });

            provinceData.Name = province.Name;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Berhasil memperbarui data provinsi" });
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
