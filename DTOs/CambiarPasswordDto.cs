namespace RinconSylvanian.Api.DTOs
{
    public class CambiarPasswordDto
    {
        public Guid Token { get; set; }
        public string NuevaPassword { get; set; }
    }
}
