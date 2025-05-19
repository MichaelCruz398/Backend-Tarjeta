using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RinconSylvanian.Api.Data;
using System.Security.Claims;

namespace RinconSylvanian.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PremiosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PremiosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("mis-premios")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> ObtenerMisPremios()
        {
            var usuarioId = ObtenerUsuarioId();
            var premios = await _context.Premios
                .Where(p => p.UsuarioId == usuarioId)
                .Select(p => new {
                    p.Id, 
                    p.Descuento,
                    p.Fecha,
                    p.TarjetaId,
                    p.Usado,
                    p.EnRevision
                })
                .ToListAsync();

            return Ok(premios);
        }

        [HttpPost("solicitar-uso/{id}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> SolicitarUso(int id)
        {
            var premio = await _context.Premios.FindAsync(id);
            if (premio == null || premio.Usado || premio.EnRevision)
                return BadRequest(new { message = "Premio inválido o ya en proceso" });

            premio.EnRevision = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Solicitud enviada correctamente" });
        }
        private int ObtenerUsuarioId()
        {
            return int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
        [HttpGet("solicitudes")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> ObtenerSolicitudes()
        {
            var solicitudes = await _context.Premios
                .Include(p => p.Usuario)
                .Where(p => p.EnRevision && !p.Usado)
                .Select(p => new {
                    p.Id,
                    p.Descuento,
                    p.Fecha,
                    p.TarjetaId,
                    Usuario = p.Usuario.Nombre,
                    p.Usuario.Email
                })
                .ToListAsync();

            return Ok(solicitudes);
        }
        [HttpPost("aprobar/{id}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> AprobarPremio(int id)
        {
            var premio = await _context.Premios.FindAsync(id);
            if (premio == null || premio.Usado || !premio.EnRevision)
                return BadRequest(new { message = "No se puede aprobar este premio" });

            premio.Usado = true;
            premio.EnRevision = false;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Premio aprobado y marcado como usado" });
        }
        [HttpPost("rechazar/{id}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> RechazarPremio(int id)
        {
            var premio = await _context.Premios.FindAsync(id);
            if (premio == null || premio.Usado || !premio.EnRevision)
                return BadRequest(new { message = "No se puede rechazar este premio" });

            premio.EnRevision = false;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Solicitud rechazada" });
        }
        [HttpGet("aprobados")]
        [Authorize(Roles = "2")] // Solo admins
        public async Task<IActionResult> ObtenerAprobados()
        {
            var aprobados = await _context.Premios
                .Include(p => p.Usuario)
                .Where(p => p.Usado)
                .OrderByDescending(p => p.Fecha)
                .Select(p => new
                {
                    p.Id,
                    p.Descuento,
                    p.Fecha,
                    p.TarjetaId,
                    Usuario = p.Usuario.Nombre,
                    p.Usuario.Email
                })
                .ToListAsync();

            return Ok(aprobados);
        }


    }

}
