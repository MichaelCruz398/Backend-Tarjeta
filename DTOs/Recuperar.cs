namespace RinconSylvanian.Api.DTOs
{
    public class Recuperar
    {
        public class SolicitarRecuperacionDto
        {
            public string Email { get; set; } = string.Empty;
        }

        public class CambiarPasswordDto
        {
            public Guid Token { get; set; }
            public string NuevaPassword { get; set; } = string.Empty;
        }
    }
}
