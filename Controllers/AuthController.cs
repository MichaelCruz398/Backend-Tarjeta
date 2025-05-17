using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RinconSylvanian.Api.Data;
using RinconSylvanian.Api.DTOs;
using RinconSylvanian.Api.Models;
using RinconSylvanian.Api.Services;

namespace RinconSylvanian.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _context;
        public AuthController(ApplicationDbContext context,JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);
            if (existe)
            {
                return BadRequest(new { message = "El correo ya está registrado" });
            }

            var nuevoUsuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = 1 // usuario normal
            };

            _context.Usuarios.Add(nuevoUsuario);

            await _context.SaveChangesAsync();
            var tarjeta = new Tarjeta
            {
                UsuarioId = nuevoUsuario.Id,
                FechaCreacion = DateTime.Now,
                StickersPegados = 0,
                Completada = false
            };

            _context.Tarjetas.Add(tarjeta);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(nuevoUsuario.Email, nuevoUsuario.Rol, nuevoUsuario.Id, nuevoUsuario.Nombre);

            return Ok(new
            {
                token,
                rol = nuevoUsuario.Rol,
                nombre = nuevoUsuario.Nombre
            });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.Password))
            {
                return Unauthorized(new { message = "Credenciales incorrectas" });
            }

            var token = _jwtService.GenerateToken(usuario.Email, usuario.Rol, usuario.Id, usuario.Nombre);

            return Ok(new
            {
                token,
                rol = usuario.Rol,
                nombre = usuario.Nombre
            });
        }
        //[HttpPost("recuperar")]
        //public async Task<IActionResult> EnviarCorreoRecuperacion([FromBody] string email)
        //{
        //    var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        //    if (usuario == null)
        //        return NotFound("Correo no registrado");

        //    var token = Guid.NewGuid();
        //    _context.RecuperacionPassword.Add(new RecuperacionPassword
        //    {
        //        UsuarioId = usuario.Id,
        //        Token = token,
        //        FechaExpiracion = DateTime.UtcNow.AddHours(1)
        //    });
        //    await _context.SaveChangesAsync();

        //    var enlace = $"http://localhost:4200/cambiar-password/{token}";
        //    await _emailService.EnviarCorreo(email, "Recuperar contraseña", $"Haz clic aquí: {enlace}");

        //    return Ok();
        //}

        //[HttpPost("cambiar-password/{token}")]
        //public async Task<IActionResult> CambiarPassword(Guid token, [FromBody] string nuevaPassword)
        //{
        //    var solicitud = await _context.RecuperacionPassword.FirstOrDefaultAsync(r => r.Token == token && r.Expira > DateTime.UtcNow);
        //    if (solicitud == null) return BadRequest("Token inválido o expirado");

        //    var usuario = await _context.Usuarios.FindAsync(solicitud.UsuarioId);
        //    usuario.Password = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);

        //    _context.RecuperacionPassword.Remove(solicitud);
        //    await _context.SaveChangesAsync();

        //    return Ok();
        //}

    }
}
