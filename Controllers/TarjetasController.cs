using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RinconSylvanian.Api.Data;
using RinconSylvanian.Api.Models;
using System.Security.Claims;

namespace RinconSylvanian.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TarjetasController : ControllerBase
    {
        private static List<Tarjeta> tarjetas = new();
        private static List<Sticker> stickers = new();
        private readonly ApplicationDbContext _context;
        public TarjetasController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int ObtenerUsuarioId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet("mis-tarjetas")]
        [Authorize(Roles = "1")] // 1 = usuario
        public async Task<IActionResult> ObtenerMisTarjetas()
        {
            var usuarioId = ObtenerUsuarioId();
            var misTarjetas = await _context.Tarjetas
                .Where(t => t.UsuarioId == usuarioId)
                .OrderByDescending(t => t.FechaCreacion)
                .ToListAsync();

            return Ok(misTarjetas);
        }

        [HttpPost("asignar-sticker")]
        [Authorize(Roles = "2")] // 2 = admin
        public async Task<IActionResult> AsignarSticker([FromBody] int usuarioId)
        {
            _context.Stickers.Add(new Sticker
            {
                UsuarioId = usuarioId,
                Usado = false
            });

            await _context.SaveChangesAsync();
            return Ok(new { message = "Sticker asignado correctamente" });
        }


        [HttpGet("mis-stickers")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> ObtenerMisStickers()
        {
            var usuarioId = ObtenerUsuarioId();
            var disponibles = await _context.Stickers
                .Where(s => s.UsuarioId == usuarioId && !s.Usado)
                .ToListAsync();

            return Ok(new { cantidad = disponibles.Count });
        }


        [HttpPost("pegar-sticker")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> PegarSticker([FromBody] int tarjetaId)
        {
            var usuarioId = ObtenerUsuarioId();

            var sticker = await _context.Stickers.FirstOrDefaultAsync(s => s.UsuarioId == usuarioId && !s.Usado);
            var tarjeta = await _context.Tarjetas.FirstOrDefaultAsync(t => t.Id == tarjetaId && t.UsuarioId == usuarioId && !t.Completada);

            if (sticker == null || tarjeta == null)
                return BadRequest(new { message = "No se pudo pegar el sticker" });

            sticker.Usado = true;
            tarjeta.StickersPegados++;

            // Reglas de premio
            if (tarjeta.StickersPegados % 3 == 0)
            {
                var descuentos = new Dictionary<int, int> {
                    { 3, 10 }, { 6, 10 }, { 9, 15 }, { 12, 15 }, { 15, 20 }
                };

                var descuento = descuentos[tarjeta.StickersPegados];

                _context.Premios.Add(new Premio
                {
                    UsuarioId = usuarioId,
                    TarjetaId = tarjetaId,
                    Descuento = descuento,
                    Fecha = DateTime.Now
                });
            }

            if (tarjeta.StickersPegados >= 15)
            {
                tarjeta.Completada = true;

                // Crear nueva tarjeta
                _context.Tarjetas.Add(new Tarjeta
                {
                    UsuarioId = usuarioId,
                    FechaCreacion = DateTime.Now,
                    StickersPegados = 0,
                    Completada = false
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Sticker pegado correctamente" });
        }
        [HttpGet("completadas")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> ObtenerTarjetasCompletadas()
        {
            var usuarioId = ObtenerUsuarioId();
            var tarjetas = await _context.Tarjetas
                .Where(t => t.UsuarioId == usuarioId && t.Completada)
                .OrderByDescending(t => t.FechaCreacion)
                .ToListAsync();

            return Ok(tarjetas);
        }
    }
}