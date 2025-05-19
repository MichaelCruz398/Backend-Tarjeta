using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RinconSylvanian.Api.Data;

namespace RinconSylvanian.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("todos")]
        [Authorize(Roles = "2")] // Solo admins pueden ver la lista
        public async Task<IActionResult> ObtenerTodos()
        {
            var usuarios = await _context.Usuarios
                 .Where(u => u.Rol == 1)
                 .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Email,
                    u.Rol,
                    Stickers = _context.Stickers.Where(s => s.UsuarioId == u.Id && !s.Usado).Count()
                })
                .ToListAsync();

            return Ok(usuarios);
        }
    }
}
