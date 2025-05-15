using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RinconSylvanian.Api.Models;

namespace RinconSylvanian.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TarjetasController : ControllerBase
    {
        private static List<Tarjeta> tarjetas = new();
        private static List<Sticker> stickers = new();

        [HttpGet("mis-tarjetas")]
        [Authorize(Roles = "usuario")]
        public IActionResult ObtenerMisTarjetas()
        {
            var usuarioId = 1; // Simulación de usuario autenticado
            var misTarjetas = tarjetas.Where(t => t.UsuarioId == usuarioId).ToList();
            return Ok(misTarjetas);
        }

        [HttpPost("asignar-sticker")]
        [Authorize(Roles = "admin")]
        public IActionResult AsignarSticker([FromBody] int usuarioId)
        {
            stickers.Add(new Sticker
            {
                Id = stickers.Count + 1,
                UsuarioId = usuarioId,
                Usado = false
            });

            return Ok(new { message = "Sticker asignado correctamente" });
        }

        [HttpGet("mis-stickers")]
        [Authorize(Roles = "usuario")]
        public IActionResult ObtenerMisStickers()
        {
            var usuarioId = 1; // Simulado
            var disponibles = stickers.Where(s => s.UsuarioId == usuarioId && !s.Usado).ToList();
            return Ok(disponibles);
        }

        [HttpPost("pegar-sticker")]
        [Authorize(Roles = "usuario")]
        public IActionResult PegarSticker([FromBody] int tarjetaId)
        {
            var usuarioId = 1; // Simulado

            var sticker = stickers.FirstOrDefault(s => s.UsuarioId == usuarioId && !s.Usado);
            var tarjeta = tarjetas.FirstOrDefault(t => t.Id == tarjetaId && t.UsuarioId == usuarioId);

            if (sticker == null || tarjeta == null)
                return BadRequest(new { message = "No se pudo pegar el sticker" });

            sticker.Usado = true;
            tarjeta.StickersPegados++;

            if (tarjeta.StickersPegados >= 15)
                tarjeta.Completada = true;

            return Ok(new { message = "Sticker pegado correctamente" });
        }
    }
}
