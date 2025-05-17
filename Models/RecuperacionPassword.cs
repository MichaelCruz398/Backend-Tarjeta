namespace RinconSylvanian.Api.Models
{
    public class RecuperacionPassword
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Guid Token { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public bool Usado { get; set; }

        public Usuario Usuario { get; set; } = null!;
    }

}
