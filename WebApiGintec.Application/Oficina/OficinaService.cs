using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiGintec.Application.Campeonato.Models;
using WebApiGintec.Application.Oficina.Models;
using WebApiGintec.Application.Util;
using WebApiGintec.Repository;

namespace WebApiGintec.Application.Oficina
{
    public class OficinaService
    {
        private GintecContext _context;
        public OficinaService(GintecContext context)
        {
            _context = context;
        }

        public GenericResponse<List<Repository.Tables.Oficina>> ObterOficinas()
        {
            try
            {
                return new GenericResponse<List<Repository.Tables.Oficina>>()
                {
                    response = _context.Oficina.Include(x => x.Calendario).Where(y => y.isAtivo == true && y.Calendario.isDeleted == false).ToList(),
                    mensagem = "success"
                };
            }
            catch (Exception ex)
            {

                return new GenericResponse<List<Repository.Tables.Oficina>>()
                {
                    mensagem = "failed",
                    error = ex
                };
            }
        }
        public GenericResponse<Repository.Tables.Oficina> ObterOficinaPorCodigo(int id)
        {
            try
            {
                return new GenericResponse<Repository.Tables.Oficina>()
                {
                    response = _context.Oficina.Include(x => x.HorariosFuncionamento).FirstOrDefault(x => x.Codigo == id && x.isAtivo == true),
                    mensagem = "success"
                };
            }
            catch (Exception ex)
            {

                return new GenericResponse<Repository.Tables.Oficina>()
                {
                    mensagem = "failed",
                    error = ex
                };
            }
        }
        public GenericResponse<Repository.Tables.AtividadeCampeonatoRealizada> ObterOficinaFeitaPorCodigo(int id, int usuariocodigo)
        {
            try
            {
                return new GenericResponse<Repository.Tables.AtividadeCampeonatoRealizada>()
                {
                    response = _context.AtividadesCampeonatoRealizadas.FirstOrDefault(x => x.UsuarioCodigo == usuariocodigo && x.OficinaCodigo == id),
                    mensagem = "success"
                };
            }
            catch (Exception ex)
            {

                return new GenericResponse<Repository.Tables.AtividadeCampeonatoRealizada>()
                {
                    mensagem = "failed",
                    error = ex
                };
            }
        }

        public GenericResponse<List<OficinaFeitasResponse>> ObterOficinasFeitos(int usuarioCodigo)
        {
            try
            {
                var lstOficina = _context.Oficina.Include(x => x.HorariosFuncionamento).Include(x => x.Calendario).Where(p => p.isAtivo == true && p.Calendario.isDeleted == false).ToList();
                var lstResponse = new List<OficinaFeitasResponse>();
                var oficinasfeitas = _context.AtividadesCampeonatoRealizadas.Where(x => x.OficinaCodigo != null && x.UsuarioCodigo == usuarioCodigo).ToList();
                foreach (var oficina in lstOficina)
                {
                    lstResponse.Add(new OficinaFeitasResponse()
                    {
                        Codigo = oficina.Codigo,
                        Data = oficina.Calendario.DataGincana,
                        Descricao = oficina.Descricao,
                        HorariosFuncionamento = oficina.HorariosFuncionamento,
                        SalaCodigo = oficina.SalaCodigo,
                        isRealizada = oficinasfeitas.Any(x => x.Oficinahorario.OficinaCodigo == oficina.Codigo),
                        OficinaHorarioFuncionamentoCodigo = oficinasfeitas.FirstOrDefault(x => x.Oficinahorario.OficinaCodigo == oficina.Codigo)?.Codigo,
                    });
                }
                return new GenericResponse<List<OficinaFeitasResponse>>()
                {
                    response = lstResponse,
                    mensagem = "success"
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<List<OficinaFeitasResponse>>()
                {
                    error = ex,
                    mensagem = "failed"
                };
            }
        }

        public GenericResponse<Repository.Tables.Oficina> AdicionarOficina(OficinaRequest request)
        {
            try
            {
                var oficinaDb = _context.Oficina.Add(new Repository.Tables.Oficina()
                {
                    CalendarioCodigo = request.CalendarioCodigo,
                    Descricao = request.Descricao,
                    SalaCodigo = request.SalaCodigo,
                    isAtivo = true
                });
                _context.SaveChanges();
                return new GenericResponse<Repository.Tables.Oficina>()
                {
                    mensagem = "success",
                    response = oficinaDb.Entity
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Oficina>()
                {
                    response = null,
                    error = ex,
                    mensagem = "failed"
                };
            }
        }
        public GenericResponse<bool> DeletarOficina(int oficinaCodigo)
        {
            try
            {
                var oficina = _context.Oficina.FirstOrDefault(o => o.Codigo == oficinaCodigo);
                if (oficina == null)
                {
                    return new GenericResponse<bool>()
                    {
                        mensagem = "Oficina não encontrada",
                        response = false
                    };
                }
                oficina.isAtivo = false;

                _context.Oficina.Update(oficina);
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
        public GenericResponse<Repository.Tables.Oficina> AtualizarOficina(int oficinaCodigo, OficinaRequest request)
        {
            try
            {
                var oficina = _context.Oficina.FirstOrDefault(o => o.Codigo == oficinaCodigo);
                if (oficina == null)
                {
                    return new GenericResponse<Repository.Tables.Oficina>()
                    {
                        mensagem = "Oficina não encontrada",
                        response = null
                    };
                }

                oficina.CalendarioCodigo = request.CalendarioCodigo;
                oficina.Descricao = request.Descricao;
                oficina.SalaCodigo = request.SalaCodigo;

                _context.Oficina.Update(oficina);
                _context.SaveChanges();

                return new GenericResponse<Repository.Tables.Oficina>()
                {
                    mensagem = "success",
                    response = oficina
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<Repository.Tables.Oficina>()
                {
                    mensagem = "failed",
                    response = null,
                    error = ex
                };
            }
        }


    }
}
