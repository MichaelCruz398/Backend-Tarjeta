using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RinconSylvanian.Api.Data;

namespace RinconSylvanian.Api.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("todos")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> ObtenerTodos()
        {

            var usuarios = await _context.Usuarios
                .Select(u => new {
                    u.Id,
                    u.Nombre,
                    u.Email,
                    u.Rol
                })
                .ToListAsync();

            return Ok(usuarios);
        }

    }
}
