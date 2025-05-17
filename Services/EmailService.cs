using SendGrid.Helpers.Mail;
using SendGrid;

namespace RinconSylvanian.Api.Services
{
    public class EmailService
    {
        private readonly string _apiKey;

        public EmailService(IConfiguration config)
        {
            _apiKey = config["SendGrid:ApiKey"];
        }

        public async Task EnviarCorreo(string destinatario, string asunto, string nombre, string url)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("michael.cruz.398@gmail.com", "Rincón Sylvanian");
            var to = new EmailAddress(destinatario);

            var html = File.ReadAllText("Templates/RecuperarPassword.html")
                .Replace("{{nombre}}", nombre)
                .Replace("{{url}}", url);

            var msg = MailHelper.CreateSingleEmail(from, to, asunto, "Recupera tu contraseña", html);
            await client.SendEmailAsync(msg);
        }

    }
}
