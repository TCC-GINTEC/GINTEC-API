using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiGintec.Application.Atividade.Models;
using WebApiGintec.Application.Util;
using WebApiGintec.Repository;
using WebApiGintec.Repository.Tables;

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
        public GenericResponse<Repository.Tables.Atividade> InserirAtividade(AtividadeRequest request)
        {
            try
            {
                var user = _context.Atividades.Add(new Repository.Tables.Atividade()
                {
                    Descricao = request.Descricao,
                    IsPontuacaoExtra = request.IsPontuacaoExtra,
                    SalaCodigo = request.SalaCodigo
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
                    SalaCodigo = request.SalaCodigo
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

                if (activity.IsPontuacaoExtra)
                {
                    var scores = _context.AtividadesPontuacaoExtra.Where(x => x.AtividadeCodigo == codigo);
                    _context.AtividadesPontuacaoExtra.RemoveRange(scores);
                }
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
        public GenericResponse<Repository.Tables.AtividadeCampeonatoRealizada> MarcarPontos(AtividadeCampeonatoRealizadaRequest request, int codigoUsuario)
        {
            try
            {
                if (_context.AtividadesCampeonatoRealizadas.Any(x => x.UsuarioCodigo == codigoUsuario &&
                (x.AtividadeCodigo == request.AtividadeCodigo || x.CampeonatoCodigo == request.CampeonatoCodigo)))
                    return new GenericResponse<AtividadeCampeonatoRealizada>() { mensagem = "Score already marked", response = null };

                var activityPerformed = new AtividadeCampeonatoRealizada();
                if (request.AtividadeCodigo == null)
                    activityPerformed = _context.AtividadesCampeonatoRealizadas.Add(new AtividadeCampeonatoRealizada
                    {
                        CampeonatoCodigo = request.CampeonatoCodigo,
                        AtividadePontuacaoExtraCodigo = request.AtividadePontuacaoExtraCodigo,
                        UsuarioCodigo = codigoUsuario,
                        dataCad = DateTime.Now
                    }).Entity;
                else
                    activityPerformed = _context.AtividadesCampeonatoRealizadas.Add(new AtividadeCampeonatoRealizada
                    {
                        AtividadeCodigo = request.AtividadeCodigo,
                        AtividadePontuacaoExtraCodigo = request.AtividadePontuacaoExtraCodigo,
                        UsuarioCodigo = codigoUsuario,
                        dataCad = DateTime.Now
                    }).Entity;

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
        #endregion
    }
}
