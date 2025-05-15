namespace RinconSylvanian.Api.Models
{
    public class Premio
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public int TarjetaId { get; set; }

        public DateTime Fecha { get; set; }

        public int Descuento { get; set; }

        // Relaciones (opcional para navegación)
        public Usuario Usuario { get; set; }
        public Tarjeta Tarjeta { get; set; }
    }
}
