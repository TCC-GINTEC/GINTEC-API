using Microsoft.EntityFrameworkCore;
using WebApiGintec.Application.Campeonato.Models;
using WebApiGintec.Application.Util;
using WebApiGintec.Repository;
using WebApiGintec.Repository.Tables;

namespace WebApiGintec.Application.Campeonato
{
    public class CampeonatoService
    {
        private readonly GintecContext _context;

        public CampeonatoService(GintecContext context)
        {
            _context = context;
        }

        public GenericResponse<Repository.Tables.Campeonato> CriarCampeonato(Repository.Tables.Campeonato campeonato)
        {
            try
            {
                var result = _context.Campeonatos.Add(campeonato);
                _context.SaveChanges();
                return new GenericResponse<Repository.Tables.Campeonato>
                {
                    mensagem = "success",
                    response = result.Entity
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Campeonato>
                {
                    mensagem = "failed",
                    error = ex
                };
            }
        }

        public GenericResponse<Repository.Tables.Campeonato> AtualizarCampeonato(int codigoCampeonato, CampeonatoRequest campeonato)
        {
            try
            {
                var existingCampeonato = _context.Campeonatos.Find(codigoCampeonato);
                if (existingCampeonato == null)
                {
                    return new GenericResponse<Repository.Tables.Campeonato>
                    {
                        mensagem = "not found"
                    };
                }

                existingCampeonato.Descricao = campeonato.Descricao;
                existingCampeonato.SalaCodigo = campeonato.SalaCodigo;

                _context.Campeonatos.Update(existingCampeonato);
                _context.SaveChanges();

                return new GenericResponse<Repository.Tables.Campeonato>
                {
                    mensagem = "success",
                    response = existingCampeonato
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Campeonato>
                {
                    mensagem = "failed",
                    error = ex
                };
            }
        }

        public GenericResponse<bool> DeletarCampeonato(int codigoCampeonato)
        {
            try
            {
                var campeonato = _context.Campeonatos.Find(codigoCampeonato);
                if (campeonato == null)
                {
                    return new GenericResponse<bool>
                    {
                        mensagem = "not found",
                        response = false
                    };
                }

                _context.Campeonatos.Remove(campeonato);
                _context.SaveChanges();

                return new GenericResponse<bool>
                {
                    mensagem = "success",
                    response = true
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>
                {
                    mensagem = "failed",
                    error = ex,
                    response = false
                };
            }
        }

        public GenericResponse<Repository.Tables.Campeonato> ObterCampeonatoPorCodigo(int codigoCampeonato)
        {
            try
            {
                var campeonato = _context.Campeonatos
                            .Include(c => c.Fases)
                            .ThenInclude(y => y.atividadeCampeonatoRealizadas)
                            .FirstOrDefault(c => c.Codigo == codigoCampeonato);

                if (campeonato == null)
                {
                    return new GenericResponse<Repository.Tables.Campeonato>
                    {
                        mensagem = "not found"
                    };
                }

                return new GenericResponse<Repository.Tables.Campeonato>
                {
                    mensagem = "success",
                    response = campeonato
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Campeonato>
                {
                    mensagem = "failed",
                    error = ex
                };
            }
        }
        public GenericResponse<List<CampeonatoFeitoResponse>> ObterCampeonatoFeito(int codigoCampeonato, int usuarioCodigo)
        {
            try
            {

                var fasescampeonato = _context.Fases.Where(x => x.CampeonatoCodigo == codigoCampeonato).ToList();
                if (fasescampeonato.Count == 0)
                {
                    return new GenericResponse<List<CampeonatoFeitoResponse>>()
                    {
                        mensagem = "not found"
                    };
                }
                var fasesCodigo = fasescampeonato.Select(x => x.Codigo).ToList();

                var fasesfeitas = _context.AtividadesCampeonatoRealizadas.Where(x => fasesCodigo.Contains(x.CampeonatoCodigo ?? 0)).ToList();

                List<CampeonatoFeitoResponse> response = new List<CampeonatoFeitoResponse>();

                foreach (var fase in fasescampeonato)
                {
                    var status = fasesfeitas.Any(x => x.CampeonatoCodigo == fase.Codigo) ? CampeonatoFeitoResponse.Status.Feito : CampeonatoFeitoResponse.Status.Pendente;
                    response.Add(new CampeonatoFeitoResponse()
                    {
                        CampeonatoCodigo = fase.CampeonatoCodigo,
                        Codigo = fase.Codigo,
                        Descricao = fase.Descricao,
                        Situacao = status
                    });
                }

                return new GenericResponse<List<CampeonatoFeitoResponse>>()
                {
                    mensagem = "success",
                    response = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<CampeonatoFeitoResponse>>()
                {
                    mensagem = "failed",
                    error = ex
                };
            }
        }

        public GenericResponse<List<Repository.Tables.Campeonato>> ObterCampeonatos()
        {
            try
            {
                var campeonatos = _context.Campeonatos
                    .Include(c => c.Fases)
                    .ToList();

                return new GenericResponse<List<Repository.Tables.Campeonato>>
                {
                    mensagem = "success",
                    response = campeonatos
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<Repository.Tables.Campeonato>>
                {
                    mensagem = "failed",
                    error = ex
                };
            }
        }
        public GenericResponse<bool> IniciarCampeonato(StartCampeonatoModel request)
        {
            try
            {
                var camp = _context.Campeonatos.Include(x => x.Fases).FirstOrDefault(x => x.Codigo == request.CampeonatoCodigo);
                if (camp == null)
                    return new GenericResponse<bool>()
                    {
                        mensagem = "Championship not found",
                        response = false
                    };
                if (camp.Fases.Count > 0)
                    return new GenericResponse<bool>()
                    {
                        mensagem = "Championship already started",
                        response = false
                    };

                var playersChamp = _context.CampeonatoJogador.Where(x => x.CampeonatoCodigo == camp.Codigo).ToList();
                List<List<CampeonatoJogador>> teams = new List<List<CampeonatoJogador>>();
                foreach (var player in playersChamp.GroupBy(x => x.SalaCodigo))
                {
                    var players = playersChamp.Where(x => x.SalaCodigo == player.Key);
                    foreach (var play in players.GroupBy(x => x.TimeCodigo))
                    {
                        teams.Add(players.Where(x => x.TimeCodigo == play.Key).ToList());
                    }
                }

                var rng = new Random();
                teams = teams.OrderBy(x => rng.Next()).ToList();

                int cont = teams.Count();
                List<CampeonatoFase> lstPhases = new List<CampeonatoFase>();
                for (int i = 0; i <= cont; i++)
                {
                    lstPhases.Add(new CampeonatoFase()
                    {
                        CampeonatoCodigo = camp.Codigo,
                        Descricao = $"{i + 1}° Fase"
                    });
                    float f = (float)cont / 2;
                    cont = (int)Math.Ceiling(f);
                }
                _context.Fases.AddRange(lstPhases);
                _context.SaveChanges();

                var numTeams = teams.Count();
                List<List<CampeonatoJogador>> nextPhaseTeams = new List<List<CampeonatoJogador>>();

                for (int i = 0; i < ((numTeams % 2) == 0 ? numTeams : numTeams - 1); i += 2)
                {
                    if (i + 1 < numTeams)
                    {
                        _context.Jogos.Add(new CampeonatoJogo()
                        {
                            DataJogo = request.DataInicio.AddMinutes(i * 15),
                            FaseCodigo = lstPhases.First().Codigo,
                            Sala1Codigo = teams[i].First().SalaCodigo,
                            Sala2Codigo = teams[i + 1].First().SalaCodigo,
                            TimeCodigo1 = teams[i].First().TimeCodigo,
                            TimeCodigo2 = teams[i + 1].First().TimeCodigo
                        });
                    }
                }

                if ((numTeams % 2) != 0)
                {
                    _context.Jogos.Add(new CampeonatoJogo()
                    {
                        DataJogo = request.DataInicio.AddMinutes(numTeams * 15),
                        FaseCodigo = lstPhases.First().Codigo,
                        Sala1Codigo = teams.Last().First().SalaCodigo,
                        Sala2Codigo = teams.Last().First().SalaCodigo,
                        TimeCodigo1 = teams.Last().First().TimeCodigo,
                        TimeCodigo2 = teams.Last().First().TimeCodigo
                    });
                }
                _context.SaveChanges();


                return new GenericResponse<bool>()
                {
                    mensagem = "success",
                    response = true
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>()
                {
                    mensagem = "An error occurred: " + ex.Message,
                    response = false
                };
            }
        }
    }
}
