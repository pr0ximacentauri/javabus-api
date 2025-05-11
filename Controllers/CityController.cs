using javabus_api.Contexts;
using javabus_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace javabus_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase    
    {
        private readonly ApplicationDBContext _context;

        public CityController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            var cities = await _context.Cities
                .Include(c => c.Province)
                .ToListAsync();

            if (cities == null || cities.Count == 0)
                return NotFound(new { message = "Tidak ada data kota" });

            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            var city = await _context.Cities
                .Include(c => c.Province)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
                return NotFound(new { message = "Kota tidak ditemukan" });

            return Ok(city);
        }


        [HttpPost, Authorize(Roles = "admin")]
        public async Task<ActionResult<City>> CreateCity(City city)
        {
            if (string.IsNullOrWhiteSpace(city.Name))
                return BadRequest(new { message = "Nama kota tidak boleh kosong" });

            if (city.ProvinceId <= 0)
                return BadRequest(new { message = "ProvinceId tidak valid" });

            var existingProvince = await _context.Provinces.FindAsync(city.ProvinceId);
            if (existingProvince == null)
                return BadRequest(new { message = "Provinsi tidak ditemukan" });

            var duplicateCity = await _context.Cities
                .AnyAsync(c => c.Name.ToLower() == city.Name.ToLower() && c.ProvinceId == city.ProvinceId);

            if (duplicateCity)
                return Conflict(new { message = "Kota dengan nama tersebut sudah ada di provinsi ini" });

            try
            {
                _context.Cities.Add(city);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCity), new { id = city.Id }, city);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Gagal menambahkan data kota!", error = ex.Message });
            }
        }

        [HttpPut("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCity(int id, City city)
        {
            if (string.IsNullOrWhiteSpace(city.Name))
                return BadRequest(new { message = "Nama kota tidak boleh kosong" });

            if (city.ProvinceId <= 0)
                return BadRequest(new { message = "ProvinceId tidak valid" });

            var existingProvince = await _context.Provinces.FindAsync(city.ProvinceId);
            if (existingProvince == null)
                return BadRequest(new { message = "Provinsi tidak ditemukan" });

            var cityData = await _context.Cities.FindAsync(id);
            if (cityData == null)
                return NotFound(new { message = "Kota dengan id tersebut tidak ditemukan" });

            var duplicateCity = await _context.Cities
                .AnyAsync(c => c.Id != id && c.Name.ToLower() == city.Name.ToLower() && c.ProvinceId == city.ProvinceId);

            if (duplicateCity)
                return Conflict(new { message = "Kota dengan nama tersebut sudah ada di provinsi ini" });

            cityData.Name = city.Name;
            cityData.ProvinceId = city.ProvinceId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Berhasil memperbarui data kota" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Gagal memperbarui data kota!", error = ex.Message });
            }
        }


        [HttpDelete("{id}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
                return NotFound(new { message = "Kota tidak ditemukan" });

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kota berhasil dihapus" });
        }
    }
}
