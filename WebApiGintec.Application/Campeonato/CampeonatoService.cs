using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        public GenericResponse<Repository.Tables.Campeonato> CriarCampeonato(CampeonatoRequest campeonato)
        {
            try
            {
                var result = _context.Campeonatos.Add(new Repository.Tables.Campeonato()
                {
                    CalendarioCodigo = campeonato.CalendarioCodigo,
                    Descricao = campeonato.Descricao,
                    isQuadra = campeonato.isQuadra,
                    SalaCodigo = campeonato.SalaCodigo,
                    isAtivo = true
                });
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
                existingCampeonato.CalendarioCodigo = campeonato.CalendarioCodigo;
                existingCampeonato.isQuadra = campeonato.isQuadra;

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

                campeonato.isAtivo = false;
                _context.Campeonatos.Update(campeonato);
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
                            .FirstOrDefault(c => c.Codigo == codigoCampeonato && c.isAtivo == true);

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
        public GenericResponse<List<CampeonatoJogoReponse>> ObterJogosPorCodigoFase(int codigoFase)
        {
            try
            {
                var fase = _context.Fases.FirstOrDefault(x => x.Codigo == codigoFase);
                var campeonato = _context.Jogos.Include(x => x.Resultado).Where(c => c.FaseCodigo == codigoFase).ToList();

                var players = _context.CampeonatoJogador.Include(x => x.Usuario).ToList();
                var salas = _context.Salas.ToList();

                var response = new List<CampeonatoJogoReponse>();
                foreach (var camp in campeonato)
                {
                    if (camp.Resultado == null)
                    {
                        var sala1 = salas.FirstOrDefault(x => x.Codigo == camp.Sala1Codigo);
                        var sala2 = salas.FirstOrDefault(x => x.Codigo == camp.Sala2Codigo);
                        response.Add(new CampeonatoJogoReponse
                        {
                            Codigo = camp.Codigo,
                            FaseCodigo = camp.FaseCodigo,
                            Sala1Codigo = camp.Sala1Codigo,
                            Sala2Codigo = camp.Sala2Codigo,
                            DataJogo = camp.DataJogo,
                            TimeCodigo1 = camp.TimeCodigo1,
                            TimeCodigo2 = camp.TimeCodigo2,
                            Resultado = camp.Resultado != null ? new CampeonatoResultado
                            {
                                JogoCodigo = camp.Resultado.JogoCodigo,
                                SalaCodigo = camp.Resultado.SalaCodigo,
                                TimeCodigo = camp.Resultado.TimeCodigo,
                            } : null,
                            Nome1 = players.Where(x => x.CampeonatoCodigo == fase.CampeonatoCodigo && x.TimeCodigo == camp.TimeCodigo1).Count() > 1
                            ? $"{sala1.Serie}° {sala1.Descricao}"
                            : players.FirstOrDefault(x => x.CampeonatoCodigo == fase.CampeonatoCodigo && x.TimeCodigo == camp.TimeCodigo1).Usuario.Nome,
                            Nome2 = players.Where(x => x.CampeonatoCodigo == fase.CampeonatoCodigo && x.TimeCodigo == camp.TimeCodigo2).Count() > 1
                            ? $"{sala2.Serie}° {sala2.Descricao}"
                            : players.FirstOrDefault(x => x.CampeonatoCodigo == fase.CampeonatoCodigo && x.TimeCodigo == camp.TimeCodigo2).Usuario.Nome,
                        });
                    }
                    else
                    {
                        var salaganhadora = salas.FirstOrDefault(x => x.Codigo == camp.Resultado.SalaCodigo);
                        response.Add(new CampeonatoJogoReponse
                        {
                            Codigo = camp.Codigo,
                            FaseCodigo = camp.FaseCodigo,
                            Sala1Codigo = camp.Sala1Codigo,
                            Sala2Codigo = camp.Sala2Codigo,
                            DataJogo = camp.DataJogo,
                            TimeCodigo1 = camp.TimeCodigo1,
                            TimeCodigo2 = camp.TimeCodigo2,
                            Resultado = camp.Resultado != null ? new CampeonatoResultado
                            {
                                JogoCodigo = camp.Resultado.JogoCodigo,
                                SalaCodigo = camp.Resultado.SalaCodigo,
                                TimeCodigo = camp.Resultado.TimeCodigo,
                            } : null,
                            Nome1 = players.Where(x => x.CampeonatoCodigo == fase.CampeonatoCodigo && x.TimeCodigo == camp.TimeCodigo1).Count() > 1
                            ? $"{salaganhadora.Serie}° {salaganhadora.Descricao}"
                            : players.FirstOrDefault(x => x.CampeonatoCodigo == fase.CampeonatoCodigo && x.TimeCodigo == camp.TimeCodigo1).Usuario.Nome,
                            Nome2 = players.Where(x => x.CampeonatoCodigo == fase.CampeonatoCodigo && x.TimeCodigo == camp.TimeCodigo2).Count() > 1
                            ? $"{salaganhadora.Serie}° {salaganhadora.Descricao}"
                            : players.FirstOrDefault(x => x.CampeonatoCodigo == fase.CampeonatoCodigo && x.TimeCodigo == camp.TimeCodigo2).Usuario.Nome,
                            Vencedor = players.Where(x => x.CampeonatoCodigo == fase.CampeonatoCodigo && x.TimeCodigo == camp.Resultado.TimeCodigo).Count() > 1
                            ? $"{salaganhadora.Serie}° {salaganhadora.Descricao}"
                            : players.FirstOrDefault(x => x.CampeonatoCodigo == fase.CampeonatoCodigo && x.TimeCodigo == camp.Resultado.TimeCodigo).Usuario.Nome,
                        });
                    }
                }

                if (campeonato == null)
                {
                    return new GenericResponse<List<CampeonatoJogoReponse>>()
                    {
                        mensagem = "not found"
                    };
                }

                return new GenericResponse<List<CampeonatoJogoReponse>>()
                {
                    mensagem = "success",
                    response = response
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<CampeonatoJogoReponse>>()
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

        //public GenericResponse<List<Repository.Tables.Campeonato>> DefinirVencedor(int salaCodigo)
        //{
        //    try
        //    {
        //        var campeonatos = _context.Jogos.Where(x => x.Sala1Codigo == salaCodigo || x.Sala2Codigo == salaCodigo).OrderBy(i => i.Codigo).Last();

        //        return new GenericResponse<List<Repository.Tables.Campeonato>>
        //        {
        //            mensagem = "success",
        //            response = campeonatos
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new GenericResponse<List<Repository.Tables.Campeonato>>
        //        {
        //            mensagem = "failed",
        //            error = ex
        //        };
        //    }
        //}
        public GenericResponse<List<Repository.Tables.Campeonato>> ObterCampeonatos()
        {
            try
            {
                var campeonatos = _context.Campeonatos
                    .Include(c => c.Fases)
                    .Where(x => x.isAtivo == true)
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
        public GenericResponse<List<Repository.Tables.CampeonatoJogador>> ObterJogadorPorCampeonatoESala(int salacodigo, int campeonatocodigo)
        {
            try
            {
                var lst = _context.CampeonatoJogador.Include(o => o.Usuario).Where(x => x.CampeonatoCodigo == campeonatocodigo && x.SalaCodigo == salacodigo).ToList();
                return new GenericResponse<List<Repository.Tables.CampeonatoJogador>>()
                {
                    mensagem = "success",
                    response = lst
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<Repository.Tables.CampeonatoJogador>>()
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
                var camp = _context.Campeonatos.Include(x => x.Fases).FirstOrDefault(x => x.Codigo == request.CampeonatoCodigo && x.isAtivo == true);
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

        public GenericResponse<bool> DefinirVencedor(int timecodigo, int fasecodigo)
        {
            try
            {
                var jogoSala = _context.Jogos.FirstOrDefault(x => x.FaseCodigo == fasecodigo && (timecodigo == x.TimeCodigo1 || x.TimeCodigo2 == timecodigo));

                if (jogoSala == null)
                    return new GenericResponse<bool>()
                    {
                        response = false,
                        mensagem = "not found"
                    };

                if (_context.Resultados.Any(x => x.JogoCodigo == jogoSala.Codigo))
                    return new GenericResponse<bool>()
                    {
                        response = false,
                        mensagem = "scored has marked"
                    };
                _context.Resultados.Add(new CampeonatoResultado()
                {
                    JogoCodigo = jogoSala.Codigo,
                    Pontos = 350,
                    SalaCodigo = jogoSala.TimeCodigo2 == timecodigo ? jogoSala.Sala2Codigo : jogoSala.Sala1Codigo,
                    TimeCodigo = timecodigo
                });
                _context.SaveChanges();

                var fase = _context.Fases.FirstOrDefault(x => x.Codigo == fasecodigo);

                var nextfase = _context.Fases.Where(x => x.CampeonatoCodigo == fase.CampeonatoCodigo).OrderBy(i => i.Codigo).ToList()
                    .FirstOrDefault(o => o.Codigo > fasecodigo);

                if (nextfase == null)
                {
                    return new GenericResponse<bool>()
                    {
                        response = false,
                        mensagem = "champshion end"
                    };
                }

                var jogos = _context.Jogos.Include(i => i.Resultado).Where(x => x.FaseCodigo == fasecodigo && x.Resultado != null).ToList();

                var lsttimesdisponiveis = new List<CampeonatoResultado>();
                foreach (var jgocode in jogos)
                {
                    if (!_context.Jogos.Any(x => x.FaseCodigo == nextfase.Codigo && (x.TimeCodigo1 == jgocode.Resultado.TimeCodigo || x.TimeCodigo2 == jgocode.Resultado.TimeCodigo)))
                    {
                        lsttimesdisponiveis.Add(jgocode.Resultado);
                    }
                }
                if (lsttimesdisponiveis.Count == 2)
                {
                    _context.Jogos.Add(new CampeonatoJogo()
                    {
                        FaseCodigo = nextfase.Codigo,
                        DataJogo = jogoSala.DataJogo.AddMinutes(30),
                        Sala1Codigo = lsttimesdisponiveis[0].SalaCodigo,
                        Sala2Codigo = lsttimesdisponiveis[1].SalaCodigo,
                        TimeCodigo1 = lsttimesdisponiveis[0].TimeCodigo,
                        TimeCodigo2 = lsttimesdisponiveis[1].TimeCodigo,
                    });
                    _context.SaveChanges();
                }


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
                    response = false,
                    mensagem = "failed"
                };
            }
        }

        public GenericResponse<bool> CadastrarJogador(JogadorRequest request)
        {
            try
            {
                if (_context.CampeonatoJogador.Any(x => x.CampeonatoCodigo == request.CampeonatoCodigo && x.UsuarioCodigo == request.UsuarioCodigo))
                    return new GenericResponse<bool>()
                    {
                        mensagem = "has exists",
                        response = false,
                    };
                _context.CampeonatoJogador.Add(new CampeonatoJogador()
                {
                    CampeonatoCodigo = request.CampeonatoCodigo,
                    SalaCodigo = request.SalaCodigo,
                    TimeCodigo = request.TimeCodigo,
                    UsuarioCodigo = request.UsuarioCodigo
                });
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
                    mensagem = "failed",
                    error = ex,
                    response = false
                };
            }
        }

        public GenericResponse<List<JogadorResponse>> ObterJogadores(int campeonatocodigo, int salaCodigo)
        {
            try
            {
                var lst = _context.CampeonatoJogador.Include(x => x.Usuario).Where(u => u.CampeonatoCodigo == campeonatocodigo && u.SalaCodigo == salaCodigo).ToList();

                var groupTime = lst.GroupBy(x => x.TimeCodigo);

                var lstResponse = new List<JogadorResponse>();

                foreach (var t in groupTime)
                {
                    lstResponse.Add(new JogadorResponse()
                    {
                        Nomes = string.Join(", ", lst.Where(x => x.TimeCodigo == t.Key).Select(x => x.Usuario.Nome)),
                        CampeonatoCodigo = campeonatocodigo,
                        TimeCodigo = t.Key
                    });
                }
                return new GenericResponse<List<JogadorResponse>>()
                {
                    mensagem = "success",
                    response = lstResponse
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<JogadorResponse>>()
                {
                    error = ex,
                    mensagem = "failed",
                    response = null
                };
            }
        }

        public GenericResponse<bool> RemoverTime(int campeonatocodigo, int timeCodigo)
        {
            try
            {
                _context.CampeonatoJogador.RemoveRange(
                    _context.CampeonatoJogador
                    .Where(x => x.CampeonatoCodigo == campeonatocodigo && x.TimeCodigo == timeCodigo)
                    .ToList());
                _context.SaveChanges();

                return new GenericResponse<bool>()
                {
                    mensagem = "success",
                    response = false
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>()
                {
                    mensagem = "success",
                    response = false
                };
            }
        }
    }
}
