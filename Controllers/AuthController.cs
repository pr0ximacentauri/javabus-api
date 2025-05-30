﻿using javabus_api.Contexts;
using javabus_api.Helpers;
using javabus_api.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


namespace javabus_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly JwtHelper _jwt;
        public AuthController(ApplicationDBContext context, JwtHelper jwt) 
        {
            _context = context;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                return BadRequest(new {message = "username already exist!"});

            user.Password = HashPassword(user.Password);    
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Berhasil menambahkan pengguna baru" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !VerifyPassword(request.Password, user.Password))
                return Unauthorized(new { message = "Username atau password salah!" });

            var token = _jwt.GenerateToken(user);

            return Ok(new { token });
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string input, string hashed)
        {
            return HashPassword(input) == hashed;
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "User tidak ditemukan" });

            user.Username = updatedUser.Username;
            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;

            user.RoleId = 2;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Data pengguna berhasil diperbarui" });
        }

        [HttpPut("update-password/{id}")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] UpdatePasswordRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "User tidak ditemukan" });

            user.Password = HashPassword(request.NewPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password berhasil diperbarui" });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class UpdatePasswordRequest
    {
        public string NewPassword { get; set; }
    }
}
