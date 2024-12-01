using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Data;
using System.Dynamic;
using WebApiGintec.Repository.Tables;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public DbSet<ReposicaoUsuario> ReposicaoUsuario { get; set; }
        public DbSet<Notificacao> Notificacao { get; set; }
        public DbSet<AlunoPontuacao> AlunoPontuacao { get; set; }
        public DbSet<PermissaoStatus> PermissaoStatus { get; set; }
        public DbSet<Doacao> Doacao { get; set; }
        public DbSet<DoacaoAluno> DoacaoAluno { get; set; }        
        public async Task<int> ObterSomaQtdPontosAsync()
        {
            var sql = @"
    SELECT SUM(qtdpontos) AS Value
    FROM (
        SELECT SUM(atx.pontuacao) + COUNT(*) * 600 AS qtdpontos
        FROM tabatividadecampeonatorealizada AS attfeita
        LEFT JOIN tabatividadepontuacaoextra AS atx 
        ON atx.atividadecodigo = attfeita.atividadecodigo
        WHERE attfeita.atividadecodigo IS NOT NULL                  
        UNION
        SELECT COUNT(*) * 350 AS qtdpontos
        FROM tabatividadecampeonatorealizada AS attfeita
        LEFT JOIN tabatividadepontuacaoextra AS atx 
        ON atx.atividadecodigo = attfeita.atividadecodigo
        WHERE attfeita.campeonatocodigo IS NOT NULL                 
        UNION
        SELECT COUNT(*) * 600 AS qtdpontos
        FROM tabatividadecampeonatorealizada AS attfeita
        WHERE attfeita.oficinacodigo IS NOT NULL                  
    ) AS prop";

            var resultado = await this.Database
                .SqlQueryRaw<int>(sql)
                .SingleOrDefaultAsync();

            return resultado;
        }
        public async Task<int> ObterSomaQtdPontosExtrasAsync()
        {
            var sql = @"
    select 
sum(atx.pontuacao) as Value 
from tabatividadecampeonatorealizada as attfeita
left join tabatividadepontuacaoextra as atx on atx.atividadecodigo = attfeita.atividadecodigo
where attfeita.atividadecodigo is not null ";

            var resultado = await this.Database
                .SqlQueryRaw<int>(sql)
                .SingleOrDefaultAsync();

            return resultado;
        }

        public async Task<List<dynamic>> ExecutarComandoSqlAsync(string sql)
        {
            var resultado = new List<dynamic>();

            // Abrindo a conexão com o banco
            using (var connection = this.Database.GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                {
                    await connection.OpenAsync();
                }

                // Executando o comando SQL
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Lendo os dados retornados
                        while (await reader.ReadAsync())
                        {
                            var row = new ExpandoObject() as IDictionary<string, object>;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            resultado.Add(row);
                        }
                    }
                }
            }

            return resultado;
        }

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
            modelBuilder.Entity<Oficina>().ToTable("taboficinas");
            modelBuilder.Entity<Notificacao>().ToTable("tabnotificacoes");
            modelBuilder.Entity<ReposicaoUsuario>().ToTable("tabreposicaousuario");
            modelBuilder.Entity<AlunoPontuacao>().ToTable("tabalunopontuacao");
            modelBuilder.Entity<PermissaoStatus>().ToTable("tabPermissaoStatus");
            modelBuilder.Entity<Doacao>().ToTable("tabDoacao");
            modelBuilder.Entity<DoacaoAluno>().ToTable("tabDoacaoAluno");
        }

    }
}
