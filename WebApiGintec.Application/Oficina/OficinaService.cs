using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiGintec.Application.Campeonato.Models;
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
                    response = _context.Oficina.Include(x => x.HorariosFuncionamento).Include(x => x.Calendario).Where(x => x.Calendario.DataGincana.Date >= DateTime.Now.Date).ToList(),
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
                    response = _context.Oficina.Include(x => x.HorariosFuncionamento).FirstOrDefault(x => x.Codigo == id),
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
                var lstOficina = _context.Oficina.Include(x => x.HorariosFuncionamento).Include(x => x.Calendario).ToList();
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
    }
}
