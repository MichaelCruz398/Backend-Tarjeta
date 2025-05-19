namespace RinconSylvanian.Api.Models
{
    public class Premio
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int TarjetaId { get; set; }
        public int Descuento { get; set; }
        public DateTime Fecha { get; set; }

        public bool Usado { get; set; } = false;
        public bool EnRevision { get; set; } = false;

        public Usuario Usuario { get; set; }
        public Tarjeta Tarjeta { get; set; }
    }

}
