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
        public DbSet<CampeonatoFase> Fases { get; set; }
        public DbSet<CampeonatoJogo> Jogos { get; set; }
        public DbSet<CampeonatoResultado> Resultados { get; set; }
        public DbSet<Calendario> Calendario { get; set; }
        public DbSet<CampeonatoJogador> CampeonatoJogador { get; set; }
        public DbSet<OficinaHorarioFuncionamento> OficinaHorarioFuncionamento { get; set; }
        public DbSet<Oficina> Oficina { get; set; }
        public DbSet<Notificacao> Notificacao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("tabUsuario");
            modelBuilder.Entity<Sala>().ToTable("tabSala");
            modelBuilder.Entity<StatusUsuario>().ToTable("tabStatus");
            modelBuilder.Entity<Atividade>().ToTable("tabAtividade");
            modelBuilder.Entity<AtividadePontuacaoExtra>().ToTable("tabAtividadePontuacaoExtra");
            modelBuilder.Entity<AtividadeCampeonatoRealizada>().ToTable("tabAtividadeCampeonatoRealizada");
            modelBuilder.Entity<Campeonato>().ToTable("tabCampeonato");
            modelBuilder.Entity<CampeonatoFase>().ToTable("tabCampeonatoFase");
            modelBuilder.Entity<CampeonatoJogo>().ToTable("tabCampeonatoJogo");            
            modelBuilder.Entity<CampeonatoResultado>().ToTable("tabCampeonatoResultado");            
            modelBuilder.Entity<Calendario>().ToTable("tabCalendario");            
            modelBuilder.Entity<CampeonatoJogador>().ToTable("tabCampeonatoJogadores");            
            modelBuilder.Entity<OficinaHorarioFuncionamento>().ToTable("tabOficinaHorariosFuncionamento");            
            modelBuilder.Entity<Oficina>().ToTable("tabOficinas");            
            modelBuilder.Entity<Notificacao>().ToTable("tabnotificacoes");            
        }

    }
}
