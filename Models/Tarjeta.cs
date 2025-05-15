namespace RinconSylvanian.Api.Models
{
    public class Tarjeta
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int StickersPegados { get; set; }
        public bool Completada { get; set; }
    }
}
