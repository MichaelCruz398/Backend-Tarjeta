using Microsoft.AspNetCore.Mvc;
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
        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }
        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            return Ok(new { message = "Usuario registrado exitosamente." });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var usuarioSimulado = new Usuario
            {
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword("123456"), 
                Rol = 1
            };

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuarioSimulado.Password))
            {
                return Unauthorized(new { message = "Credenciales incorrectas" });
            }

            var token = _jwtService.GenerateToken(usuarioSimulado.Email, usuarioSimulado.Rol);
            return Ok(new { token, rol = usuarioSimulado.Rol });
        }
    }
}
