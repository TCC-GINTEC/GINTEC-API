using Microsoft.EntityFrameworkCore;
using WebApiGintec.Repository.Tables;

namespace WebApiGintec.Repository
{
    public class GintecContext : DbContext
    {
        public GintecContext(DbContextOptions<GintecContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<StatusUsuario> Statuses { get; set; }
        public DbSet<Atividade> Atividades { get; set; }
        public DbSet<AtividadePontuacaoExtra> AtividadesPontuacaoExtra { get; set; }
        public DbSet<AtividadeCampeonatoRealizada> AtividadesCampeonatoRealizadas { get; set; }
        public DbSet<Campeonato> Campeonatos { get; set; }        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("tabUsuario");
            modelBuilder.Entity<Sala>().ToTable("tabSala");
            modelBuilder.Entity<StatusUsuario>().ToTable("tabStatus");
            modelBuilder.Entity<Atividade>().ToTable("tabAtividade");
            modelBuilder.Entity<AtividadePontuacaoExtra>().ToTable("tabAtividadePontuacaoExtra");
            modelBuilder.Entity<AtividadeCampeonatoRealizada>().ToTable("tabAtividadeCampeonatoRealizada");
            modelBuilder.Entity<Campeonato>().ToTable("tabCampeonato");            
        }

    }
}
