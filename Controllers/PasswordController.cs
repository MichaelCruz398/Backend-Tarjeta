using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RinconSylvanian.Api.Data;
using RinconSylvanian.Api.Models;
using RinconSylvanian.Api.Services;
using static RinconSylvanian.Api.DTOs.Recuperar;

namespace RinconSylvanian.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;

        public PasswordController(ApplicationDbContext context, IConfiguration config, EmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }

        [HttpPost("solicitar-recuperacion")]
        public async Task<IActionResult> SolicitarRecuperacion([FromBody] SolicitarRecuperacionDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (usuario == null)
                return NotFound();

            var token = Guid.NewGuid();
            var recuperacion = new RecuperacionPassword
            {
                UsuarioId = usuario.Id,
                Token = token,
                FechaExpiracion = DateTime.UtcNow.AddHours(1)
            };

            _context.RecuperacionPassword.Add(recuperacion);
            await _context.SaveChangesAsync();

            var url = $"http://localhost:4200/cambiar-password?token={token}";
            var body = $"Hola {usuario.Nombre}, haz click en el siguiente enlace para cambiar tu contraseña: {url}";

            await _emailService.EnviarCorreo(usuario.Email, "Recupera tu contraseña", usuario.Nombre, url);

            return Ok(new { message = "Correo enviado" });
        }

        [HttpPost("cambiar-password")]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDto dto)
        {
            var rec = await _context.RecuperacionPassword
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r =>
                    r.Token == dto.Token &&
                    !r.Usado &&
                    r.FechaExpiracion > DateTime.UtcNow);

            if (rec == null)
            {
                Console.WriteLine("❌ No se encontró token válido en la base de datos");
                return BadRequest("Token inválido o expirado");
            }

            Console.WriteLine("✅ Token válido. Actualizando contraseña...");

            rec.Usado = true;
            rec.Usuario.Password = BCrypt.Net.BCrypt.HashPassword(dto.NuevaPassword);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Contraseña actualizada correctamente" });
        }


    }
}
