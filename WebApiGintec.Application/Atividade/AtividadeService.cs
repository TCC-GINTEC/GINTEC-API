using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApiGintec.Application.Atividade.Models;
using WebApiGintec.Application.Commom;
using WebApiGintec.Application.Util;
using WebApiGintec.Repository;
using WebApiGintec.Repository.Tables;
using ZstdSharp.Unsafe;

namespace WebApiGintec.Application.Atividade
{
    public class AtividadeService
    {
        private GintecContext _context;
        public AtividadeService(GintecContext context)
        {
            _context = context;
        }
        #region Atividades
        public GenericResponse<List<Repository.Tables.Atividade>> ObterTodosAtividades()
        {
            try
            {
                var response = new GenericResponse<List<Repository.Tables.Atividade>>()
                {
                    mensagem = "success",
                    response = _context.Atividades.Include(x => x.Sala).Include(c => c.AtividadePontuacaoExtra).ToList()
                };
                return response;
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<Repository.Tables.Atividade>>()
                {
                    mensagem = "failed",
                    response = null,
                    error = ex
                };
            }
        }
        public GenericResponse<Repository.Tables.Atividade> ObterAtividadePorCodiog(int codigo)
        {
            try
            {
                var response = new GenericResponse<Repository.Tables.Atividade>()
                {
                    mensagem = "success",
                    response = _context.Atividades.Include(x => x.Sala).Include(c => c.AtividadePontuacaoExtra).FirstOrDefault(x => x.Codigo == codigo)
                };
                return response;
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Atividade>()
                {
                    mensagem = "failed",
                    response = null,
                    error = ex
                };
            }
        }
        public GenericResponse<Repository.Tables.Atividade> InserirAtividade(AtividadeRequest request)
        {
            try
            {
                var user = _context.Atividades.Add(new Repository.Tables.Atividade()
                {
                    Descricao = request.Descricao,
                    IsPontuacaoExtra = request.IsPontuacaoExtra,
                    SalaCodigo = request.SalaCodigo,
                    CalendarioCodigo = request.CalendarioCodigo
                });
                _context.SaveChanges();

                return new GenericResponse<Repository.Tables.Atividade>()
                {
                    mensagem = "success",
                    response = user.Entity
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Atividade>()
                {
                    mensagem = "failed",
                    response = null,
                    error = ex
                };
            }
        }
        public GenericResponse<Repository.Tables.Atividade> AtualizarAtividade(int codigo, AtividadeRequest request)
        {
            try
            {
                var user = _context.Atividades.Update(new Repository.Tables.Atividade()
                {
                    Codigo = codigo,
                    Descricao = request.Descricao,
                    IsPontuacaoExtra = request.IsPontuacaoExtra,
                    SalaCodigo = request.SalaCodigo,
                    CalendarioCodigo = request.CalendarioCodigo
                });
                _context.SaveChanges();

                return new GenericResponse<Repository.Tables.Atividade>()
                {
                    mensagem = "success",
                    response = user.Entity
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Atividade>()
                {
                    mensagem = "failed",
                    response = null,
                    error = ex
                };
            }
        }
        public GenericResponse<bool> DeletarAtividade(int codigo)
        {
            try
            {
                var activity = _context.Atividades.FirstOrDefault(x => x.Codigo == codigo);
                if (activity == null)
                    return new GenericResponse<bool> { mensagem = "success", response = false };


                var scores = _context.AtividadesPontuacaoExtra.Where(x => x.AtividadeCodigo == codigo);
                _context.AtividadesPontuacaoExtra.RemoveRange(scores);
                _context.Atividades.Remove(activity);

                _context.SaveChanges();

                return new GenericResponse<bool>() { mensagem = "success", response = true };
            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>()
                {
                    mensagem = "failed",
                    response = false,
                    error = ex
                };
            }
        }
        public GenericResponse<Repository.Tables.AtividadeCampeonatoRealizada> MarcarPontos(AtividadeCampeonatoRealizadaRequest request, int usuarioAjudanteCodigo)
        {
            try
            {
                var usuario = new QRCodeService().DesencriptarQRCode(request.token);
                var usuarioDb = _context.Usuarios.FirstOrDefault(x => x.Codigo == usuario.UsuarioCodigo && x.Email == usuario.Email);
                var ajudanteDb = _context.Usuarios.FirstOrDefault(x => x.Codigo == usuarioAjudanteCodigo && (x.AtividadeCodigo != null || x.CampeonatoCodigo != null || x.oficinacodigo != null));
                var activityPerformed = new AtividadeCampeonatoRealizada();
                if (usuarioDb == null)
                    return new GenericResponse<AtividadeCampeonatoRealizada>()
                    {
                        mensagem = "user not found",
                        response = null
                    };
                if (ajudanteDb.CampeonatoCodigo != null)
                {
                    if (!_context.CampeonatoJogador.Any(x => x.CampeonatoCodigo == ajudanteDb.CampeonatoCodigo && x.UsuarioCodigo == usuarioDb.Codigo))
                    {
                        return new GenericResponse<AtividadeCampeonatoRealizada>()
                        {
                            mensagem = "Player not found in championship"
                        };
                    }
                    var fasesCampeonato = _context.Fases.Where(x => x.CampeonatoCodigo == ajudanteDb.CampeonatoCodigo).ToList();
                    var fasesCodigo = fasesCampeonato.Select(y => y.Codigo).ToList();

                    var phasesDone = _context.AtividadesCampeonatoRealizadas.Where(x => x.UsuarioCodigo == usuarioDb.Codigo && fasesCodigo.Contains(x.CampeonatoCodigo ?? 0)).ToList();

                    var jogppints = phasesDone.Sum(o => o.CampeonatoCodigo);

                    if (jogppints == fasesCodigo.Sum())
                        return new GenericResponse<AtividadeCampeonatoRealizada>() { mensagem = "Score already marked", response = null };

                    var fasequevaisermarcada = fasesCampeonato.Where(u => !phasesDone.Any(i => i.CampeonatoCodigo == u.Codigo)).OrderBy(x => x.Codigo).First();

                    activityPerformed = _context.AtividadesCampeonatoRealizadas.Add(new AtividadeCampeonatoRealizada
                    {
                        CampeonatoCodigo = fasequevaisermarcada.Codigo,
                        UsuarioCodigo = usuarioDb.Codigo,
                        dataCad = DateTime.Now
                    }).Entity;
                }
                else if (ajudanteDb.oficinacodigo != null)
                {
                    if (_context.AtividadesCampeonatoRealizadas.Any(x => x.OficinaCodigo == ajudanteDb.oficinacodigo && usuarioDb.Codigo == x.UsuarioCodigo))
                        return new GenericResponse<AtividadeCampeonatoRealizada>() { mensagem = "Score already marked", response = null };
                    _context.AtividadesCampeonatoRealizadas.Add(new AtividadeCampeonatoRealizada()
                    {
                        OficinaCodigo = ajudanteDb.oficinacodigo,
                        UsuarioCodigo = usuarioDb.Codigo,
                        dataCad = DateTime.Now,
                        Oficinahorariocodigo = request.OficinaHorarioCodigo
                    });
                    _context.SaveChanges();
                }
                else if (ajudanteDb.AtividadeCodigo != null)
                {
                    var dateNow = DateTime.Now;

                    if (_context.Atividades
                  .Any(x => x.Codigo == ajudanteDb.AtividadeCodigo &&
                  x.Calendario.DataGincana.Date != dateNow.Date))
                    {
                        return new GenericResponse<AtividadeCampeonatoRealizada>()
                        {
                            mensagem = "Activity cannot be scheduled on this day"
                        };
                    }
                    if (_context.AtividadesCampeonatoRealizadas.Any(x => x.AtividadeCodigo == ajudanteDb.AtividadeCodigo && usuarioDb.Codigo == x.UsuarioCodigo))
                    {
                        var atts = _context.Atividades.ToList();
                        var att = atts.FirstOrDefault(x => x.Codigo == ajudanteDb.AtividadeCodigo);
                        var repos = _context.ReposicaoUsuario.Where(x => x.UsuarioCodigo == usuarioDb.Codigo).ToList();

                        foreach (var repo in repos)
                        {
                            var attrepo = atts.FirstOrDefault(x => x.Descricao == att.Descricao && x.CalendarioCodigo == repo.CalendarioCodigo);

                            if (!_context.AtividadesCampeonatoRealizadas.Any(x => x.AtividadeCodigo == attrepo.Codigo && usuarioDb.Codigo == x.UsuarioCodigo))
                            {
                                activityPerformed = _context.AtividadesCampeonatoRealizadas.Add(new AtividadeCampeonatoRealizada
                                {
                                    AtividadeCodigo = attrepo.Codigo,
                                    UsuarioCodigo = usuarioDb.Codigo,
                                    dataCad = DateTime.Now
                                }).Entity;

                                _context.SaveChanges();
                                return new GenericResponse<AtividadeCampeonatoRealizada>()
                                {
                                    mensagem = "success",
                                    response = activityPerformed
                                };
                            }
                        }
                        return new GenericResponse<AtividadeCampeonatoRealizada>() { mensagem = "Score already marked", response = null };
                    }
                    activityPerformed = _context.AtividadesCampeonatoRealizadas.Add(new AtividadeCampeonatoRealizada
                    {
                        AtividadeCodigo = ajudanteDb.AtividadeCodigo,
                        AtividadePontuacaoExtraCodigo = request.AtividadePontuacaoExtraCodigo,
                        UsuarioCodigo = usuarioDb.Codigo,
                        dataCad = DateTime.Now
                    }).Entity;
                }

                _context.SaveChanges();
                return new GenericResponse<AtividadeCampeonatoRealizada>()
                {
                    mensagem = "success",
                    response = activityPerformed
                };

            }
            catch (Exception ex)
            {
                return new GenericResponse<AtividadeCampeonatoRealizada>()
                {
                    mensagem = "failed",
                    error = ex,
                    response = null
                };
            }

        }
        public GenericResponse<Repository.Tables.AtividadeCampeonatoRealizada> MarcarPontos2(AtividadeCampeonatoRealizadaRequest request, int usuarioCodigo)
        {
            try
            {
                var usuarioDb = _context.Usuarios.FirstOrDefault(x => x.RM == request.token);
                if (usuarioDb == null)
                    return new GenericResponse<AtividadeCampeonatoRealizada>() { mensagem = "user not found", response = null };
                var ajudanteDb = _context.Usuarios.FirstOrDefault(x => x.Codigo == usuarioCodigo && (x.AtividadeCodigo != null || x.CampeonatoCodigo != null));
                if (_context.AtividadesCampeonatoRealizadas.Any(x => x.UsuarioCodigo == usuarioDb.Codigo &&
                (x.AtividadeCodigo == ajudanteDb.AtividadeCodigo || x.CampeonatoCodigo == ajudanteDb.CampeonatoCodigo)))
                    return new GenericResponse<AtividadeCampeonatoRealizada>() { mensagem = "Score already marked", response = null };

                var activityPerformed = new AtividadeCampeonatoRealizada();
                if (ajudanteDb.CampeonatoCodigo != null)
                    activityPerformed = _context.AtividadesCampeonatoRealizadas.Add(new AtividadeCampeonatoRealizada
                    {
                        CampeonatoCodigo = ajudanteDb.CampeonatoCodigo,
                        AtividadePontuacaoExtraCodigo = request.AtividadePontuacaoExtraCodigo,
                        UsuarioCodigo = usuarioDb.Codigo,
                        dataCad = DateTime.Now
                    }).Entity;
                else
                    activityPerformed = _context.AtividadesCampeonatoRealizadas.Add(new AtividadeCampeonatoRealizada
                    {
                        AtividadeCodigo = ajudanteDb.AtividadeCodigo,
                        AtividadePontuacaoExtraCodigo = request.AtividadePontuacaoExtraCodigo,
                        UsuarioCodigo = usuarioDb.Codigo,
                        dataCad = DateTime.Now
                    }).Entity;

                _context.SaveChanges();

                activityPerformed.Usuario = _context.Usuarios.Find(activityPerformed.UsuarioCodigo);
                return new GenericResponse<AtividadeCampeonatoRealizada>()
                {
                    mensagem = "success",
                    response = activityPerformed
                };

            }
            catch (Exception ex)
            {
                return new GenericResponse<AtividadeCampeonatoRealizada>()
                {
                    mensagem = "failed",
                    error = ex,
                    response = null
                };
            }

        }
        #endregion
        #region Pontuação Extras
        public GenericResponse<List<Repository.Tables.AtividadePontuacaoExtra>> ObterPontuacaoExtraPorAtividade(int codigo)
        {
            try
            {
                return new GenericResponse<List<Repository.Tables.AtividadePontuacaoExtra>>
                {
                    mensagem = "success",
                    response = _context.AtividadesPontuacaoExtra.Where(x => x.AtividadeCodigo == codigo).ToList()
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<Repository.Tables.AtividadePontuacaoExtra>>
                {
                    mensagem = "failed",
                    response = null,
                    error = ex
                };
            }
        }
        public GenericResponse<Repository.Tables.AtividadePontuacaoExtra> InserirPontuacaoExtra(PontuacaoExtraRequest request)
        {
            try
            {
                var score = _context.AtividadesPontuacaoExtra.Add(new AtividadePontuacaoExtra()
                {
                    AtividadeCodigo = request.AtividadeCodigo,
                    Pontuacao = request.Pontuacao
                });
                _context.SaveChanges();

                return new GenericResponse<Repository.Tables.AtividadePontuacaoExtra>()
                {
                    mensagem = "success",
                    response = score.Entity
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.AtividadePontuacaoExtra>()
                {
                    mensagem = "failed",
                    response = null,
                    error = ex
                };
            }
        }
        public GenericResponse<bool> AtualizarPontuacaoExtra(PontuacaoExtraRequestLote request)
        {
            try
            {
                var scores = _context.AtividadesPontuacaoExtra.Where(x => x.AtividadeCodigo == request.AtividadeCodigo).ToList();
                foreach(var score in scores)
                {
                    if(request.Pontuacao.Count <= 0 || request.Pontuacao.Any(x => x != score.Pontuacao))
                    {
                        _context.AtividadesPontuacaoExtra.Remove(score);
                    }
                }
                foreach(var score in request.Pontuacao)
                {
                    if (scores.Count <= 0 || scores.Any(x => x.Pontuacao != score))
                    {
                        _context.AtividadesPontuacaoExtra.Add(new AtividadePontuacaoExtra()
                        {
                            AtividadeCodigo = request.AtividadeCodigo,
                            Pontuacao = score,
                        });
                    }
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
                    mensagem = "failed",
                    response = false,
                    error = ex
                };
            }
        }
        public GenericResponse<bool> DeletarPontuacaoExtra(int id)
        {
            try
            {
                var score = _context.AtividadesPontuacaoExtra.FirstOrDefault(x => x.Codigo == id);
                if (score == null)
                    return new GenericResponse<bool>() { mensagem = "success", response = false };
                _context.AtividadesPontuacaoExtra.Remove(score);
                _context.SaveChanges();

                return new GenericResponse<bool>() { mensagem = "success", response = true };

            }
            catch (Exception ex)
            {
                return new GenericResponse<bool>()
                {
                    mensagem = "failed",
                    response = false,
                    error = ex
                };
            }
        }

        public GenericResponse<List<AtividadesFeitasResponse>> ObterAtividadesFeitas(int usuarioCodigo)
        {
            try
            {
                var lstattFeitas = _context.AtividadesCampeonatoRealizadas.Include(x => x.AtividadePontuacaoExtra).Where(x => x.UsuarioCodigo == usuarioCodigo && x.AtividadeCodigo != null).ToList();
                var dateNow = DateTime.Now;
                var lstatt = _context.Atividades.Include(x => x.Sala).Include(c => c.AtividadePontuacaoExtra).Include(i => i.Calendario)
                    .Where(x => x.Calendario.DataGincana.Year == dateNow.Year &&
                    x.Calendario.DataGincana.Month == dateNow.Month &&
                    x.Calendario.DataGincana.Day == dateNow.Day)
                    .ToList();
                List<AtividadesFeitasResponse> responseBody = new();
                foreach (var item in lstatt)
                {
                    responseBody.Add(new AtividadesFeitasResponse()
                    {
                        Codigo = item.Codigo,
                        Descricao = item.Descricao,
                        AtividadePontuacaoExtra = item.AtividadePontuacaoExtra,
                        IsPontuacaoExtra = item.IsPontuacaoExtra,
                        Sala = item.Sala,
                        SalaCodigo = item.SalaCodigo,
                        isRealizada = lstattFeitas.Any(x => x.AtividadeCodigo == item.Codigo),
                        pontuacaoExtra = lstattFeitas.FirstOrDefault(x => x.AtividadeCodigo == item.Codigo && x.AtividadePontuacaoExtraCodigo != null)?.AtividadePontuacaoExtra.Pontuacao
                    });
                }


                var response = new GenericResponse<List<AtividadesFeitasResponse>>()
                {
                    mensagem = "success",
                    response = responseBody
                };
                return response;
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<AtividadesFeitasResponse>>()
                {
                    mensagem = "failed",
                    response = null,
                    error = ex
                };
            }
        }
        #endregion
    }
}
