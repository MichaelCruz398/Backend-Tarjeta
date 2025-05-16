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
        public async Task<IActionResult> MisPremios()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var premios = await _context.Premios
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.Fecha)
                .Select(p => new {
                    p.Fecha,
                    p.Descuento,
                    p.TarjetaId
                })
                .ToListAsync();

            return Ok(premios);
        }
    }

}
