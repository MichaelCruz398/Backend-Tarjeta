using RinconSylvanian.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace RinconSylvanian.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Sticker> Stickers { get; set; }
        public DbSet<Tarjeta> Tarjetas { get; set; }
    }
}
